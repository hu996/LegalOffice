using LegalOffice.Application.ViewModels;
using LegalOffice.Domain.Entities;
using LegalOffice.Infrastructure.Persistence;
using LegalOffice.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class MeetingsController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;
    private readonly ICaseTypeOptionsService _caseTypeOptions;

    public MeetingsController(AppDbContext db, IPermissionService permissions, ICaseTypeOptionsService caseTypeOptions)
    {
        _db = db;
        _permissions = permissions;
        _caseTypeOptions = caseTypeOptions;
    }

    public async Task<IActionResult> Index(string? search)
    {
        if (!await _permissions.HasPermissionAsync(User, "Meetings.View"))
        {
            return Forbid();
        }

        var query = _db.Meetings
            .AsNoTracking()
            .Include(x => x.Client)
            .Include(x => x.Case)
            .Include(x => x.AssignedUser)
            .Include(x => x.StatusLookup)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Subject.Contains(search) ||
                (x.Client != null && x.Client.FullName.Contains(search)) ||
                (x.Case != null && x.Case.CaseNumber.Contains(search)) ||
                x.AssignedUser.FullName.Contains(search));
        }

        ViewBag.Search = search;
        return View(await query.OrderByDescending(x => x.MeetingDate).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        if (!await _permissions.HasPermissionAsync(User, "Meetings.Create"))
        {
            return Forbid();
        }

        var vm = new MeetingVM { MeetingDate = DateTime.Now };
        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MeetingVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "Meetings.Create"))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            await Fill(vm);
            return View(vm);
        }

        if (!await ValidateCaseTypeMatchAsync(vm.CaseId, vm.CaseTypeId))
        {
            await Fill(vm);
            return View(vm);
        }

        _db.Meetings.Add(new Meeting
        {
            Subject = vm.Subject.Trim(),
            ClientId = vm.ClientId,
            CaseId = vm.CaseId,
            AssignedUserId = vm.AssignedUserId,
            MeetingDate = vm.MeetingDate,
            MeetingResult = vm.MeetingResult,
            Notes = vm.Notes,
            StatusLookupId = vm.StatusLookupId
        });
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تم حفظ الاجتماع بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!await _permissions.HasPermissionAsync(User, "Meetings.Edit"))
        {
            return Forbid();
        }

        var entity = await _db.Meetings.Include(x => x.Case).FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        var vm = new MeetingVM
        {
            Id = entity.Id,
            Subject = entity.Subject,
            ClientId = entity.ClientId,
            CaseId = entity.CaseId,
            CaseTypeId = entity.Case?.CaseTypeId,
            AssignedUserId = entity.AssignedUserId,
            MeetingDate = entity.MeetingDate,
            MeetingResult = entity.MeetingResult,
            Notes = entity.Notes,
            StatusLookupId = entity.StatusLookupId
        };

        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MeetingVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "Meetings.Edit"))
        {
            return Forbid();
        }

        if (!vm.Id.HasValue)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await Fill(vm);
            return View(vm);
        }

        if (!await ValidateCaseTypeMatchAsync(vm.CaseId, vm.CaseTypeId))
        {
            await Fill(vm);
            return View(vm);
        }

        var entity = await _db.Meetings.FirstOrDefaultAsync(x => x.Id == vm.Id.Value);
        if (entity == null)
        {
            return NotFound();
        }

        entity.Subject = vm.Subject.Trim();
        entity.ClientId = vm.ClientId;
        entity.CaseId = vm.CaseId;
        entity.AssignedUserId = vm.AssignedUserId;
        entity.MeetingDate = vm.MeetingDate;
        entity.MeetingResult = vm.MeetingResult;
        entity.Notes = vm.Notes;
        entity.StatusLookupId = vm.StatusLookupId;
        await _db.SaveChangesAsync();

        TempData["ToastSuccess"] = "تم تحديث الاجتماع بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    private async Task Fill(MeetingVM vm)
    {
        vm.Clients = await _db.Clients.AsNoTracking().OrderBy(x => x.FullName)
            .Select(x => new SelectListItem(x.FullName, x.Id.ToString())).ToListAsync();
        vm.CaseTypes = await _caseTypeOptions.GetVisibleCaseTypesAsync(User);
        vm.Cases = await BuildCasesAsync(vm.CaseTypeId);
        vm.Users = await _db.Users.AsNoTracking().OrderBy(x => x.FullName)
            .Select(x => new SelectListItem(x.FullName, x.Id)).ToListAsync();
        vm.Statuses = await _db.Lookups.Where(x => x.Type == "MeetingStatus" && x.IsActive).OrderBy(x => x.NameAr)
            .Select(x => new SelectListItem(x.NameAr, x.Id.ToString())).ToListAsync();
    }

    private async Task<List<SelectListItem>> BuildCasesAsync(int? caseTypeId)
    {
        var query = _db.Cases.AsNoTracking().Include(x => x.Client).AsQueryable();
        if (caseTypeId.HasValue)
        {
            query = query.Where(x => x.CaseTypeId == caseTypeId.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new SelectListItem($"{x.CaseNumber} - {x.Client.FullName}", x.Id.ToString()))
            .ToListAsync();
    }

    private async Task<bool> ValidateCaseTypeMatchAsync(int? caseId, int? caseTypeId)
    {
        if (!caseId.HasValue || !caseTypeId.HasValue)
        {
            return true;
        }

        var matches = await _db.Cases.AnyAsync(x => x.Id == caseId.Value && x.CaseTypeId == caseTypeId.Value);
        if (!matches)
        {
            ModelState.AddModelError(nameof(MeetingVM.CaseId), "القضية المختارة لا تطابق نوع القضية.");
            TempData["ToastError"] = "القضية المختارة لا تطابق نوع القضية.";
        }

        return matches;
    }
}
