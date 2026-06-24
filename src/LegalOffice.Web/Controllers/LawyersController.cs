using LegalOffice.Application.ViewModels;
using LegalOffice.Domain.Entities;
using LegalOffice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class LawyersController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public LawyersController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 50);

        var query = _db.Lawyers
            .Include(x => x.User)
            .Include(x => x.Specialties).ThenInclude(x => x.CaseType)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.FullName.Contains(search) ||
                (x.Email ?? "").Contains(search) ||
                (x.JobTitle ?? "").Contains(search));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(x => x.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Search = search;
        return View(new PagedResult<Lawyer>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }

    public async Task<IActionResult> Create()
    {
        var vm = new LawyerCreateEditVM();
        await FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(LawyerCreateEditVM vm)
    {
        NormalizeSpecialties(vm);
        ValidateSpecialties(vm);

        if (!ModelState.IsValid)
        {
            await FillLookups(vm);
            return View(vm);
        }

        var lawyer = new Lawyer
        {
            FullName = vm.FullName,
            Phone = vm.Phone,
            Email = vm.Email,
            JobTitle = vm.JobTitle,
            IsActive = vm.IsActive
        };

        _db.Lawyers.Add(lawyer);
        await _db.SaveChangesAsync();

        await SaveSpecialtiesAsync(lawyer.Id, vm.SelectedCaseTypeIds);
        await EnsurePortalAccountAsync(lawyer, vm);
        TempData["ToastSuccess"] = "تمت إضافة المحامي بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var lawyer = await _db.Lawyers
            .Include(x => x.Specialties)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (lawyer == null)
        {
            return NotFound();
        }

        var vm = new LawyerCreateEditVM
        {
            Id = lawyer.Id,
            FullName = lawyer.FullName,
            Phone = lawyer.Phone,
            Email = lawyer.Email,
            JobTitle = lawyer.JobTitle,
            IsActive = lawyer.IsActive,
            PortalEmail = lawyer.Email,
            SelectedCaseTypeIds = lawyer.Specialties.Select(x => x.CaseTypeId).ToList()
        };

        await FillLookups(vm);
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(LawyerCreateEditVM vm)
    {
        if (!vm.Id.HasValue)
        {
            return BadRequest();
        }

        NormalizeSpecialties(vm);
        ValidateSpecialties(vm);

        if (!ModelState.IsValid)
        {
            await FillLookups(vm);
            return View(vm);
        }

        var lawyer = await _db.Lawyers.FirstOrDefaultAsync(x => x.Id == vm.Id.Value);
        if (lawyer == null)
        {
            return NotFound();
        }

        lawyer.FullName = vm.FullName;
        lawyer.Phone = vm.Phone;
        lawyer.Email = vm.Email;
        lawyer.JobTitle = vm.JobTitle;
        lawyer.IsActive = vm.IsActive;

        await _db.SaveChangesAsync();

        await SaveSpecialtiesAsync(lawyer.Id, vm.SelectedCaseTypeIds);
        await EnsurePortalAccountAsync(lawyer, vm);
        TempData["ToastSuccess"] = "تم تعديل بيانات المحامي بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    private void NormalizeSpecialties(LawyerCreateEditVM vm)
    {
        vm.SelectedCaseTypeIds = vm.SelectedCaseTypeIds?.Where(x => x > 0).Distinct().ToList() ?? new List<int>();
    }

    private void ValidateSpecialties(LawyerCreateEditVM vm)
    {
        if (vm.SelectedCaseTypeIds.Count == 0)
        {
            ModelState.AddModelError(nameof(vm.SelectedCaseTypeIds), "اختار تخصص واحد على الأقل للمحامي.");
            TempData["ToastError"] = "اختار تخصص واحد على الأقل للمحامي.";
        }
    }

    private async Task SaveSpecialtiesAsync(int lawyerId, List<int> selectedCaseTypeIds)
    {
        var existing = await _db.LawyerSpecialties.Where(x => x.LawyerId == lawyerId).ToListAsync();
        _db.LawyerSpecialties.RemoveRange(existing);

        foreach (var caseTypeId in selectedCaseTypeIds)
        {
            _db.LawyerSpecialties.Add(new LawyerSpecialty
            {
                LawyerId = lawyerId,
                CaseTypeId = caseTypeId
            });
        }

        await _db.SaveChangesAsync();
    }

    private async Task FillLookups(LawyerCreateEditVM vm)
    {
        vm.CaseTypes = await _db.Lookups
            .Where(x => x.Type == "CaseType" && x.IsActive)
            .OrderBy(x => x.NameAr)
            .Select(x => new SelectListItem(x.NameAr, x.Id.ToString()))
            .ToListAsync();
    }

    private async Task EnsurePortalAccountAsync(Lawyer lawyer, LawyerCreateEditVM vm)
    {
        if (string.IsNullOrWhiteSpace(vm.PortalEmail))
        {
            return;
        }

        var user = string.IsNullOrWhiteSpace(lawyer.UserId)
            ? await _userManager.FindByEmailAsync(vm.PortalEmail)
            : await _userManager.FindByIdAsync(lawyer.UserId);

        if (user == null)
        {
            var password = string.IsNullOrWhiteSpace(vm.TemporaryPassword) ? "123456" : vm.TemporaryPassword;
            user = new ApplicationUser
            {
                UserName = vm.PortalEmail,
                Email = vm.PortalEmail,
                FullName = lawyer.FullName,
                LawyerId = lawyer.Id,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Lawyer");
            }
        }
        else
        {
            user.UserName = vm.PortalEmail;
            user.Email = vm.PortalEmail;
            user.FullName = lawyer.FullName;
            user.LawyerId = lawyer.Id;
            await _userManager.UpdateAsync(user);
            if (!await _userManager.IsInRoleAsync(user, "Lawyer"))
            {
                await _userManager.AddToRoleAsync(user, "Lawyer");
            }
        }

        if (user != null)
        {
            lawyer.UserId = user.Id;
            await _db.SaveChangesAsync();
        }
    }
}
