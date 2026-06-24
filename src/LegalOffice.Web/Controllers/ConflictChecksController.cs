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
public class ConflictChecksController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;
    private readonly ICaseTypeOptionsService _caseTypeOptions;

    public ConflictChecksController(AppDbContext db, IPermissionService permissions, ICaseTypeOptionsService caseTypeOptions)
    {
        _db = db;
        _permissions = permissions;
        _caseTypeOptions = caseTypeOptions;
    }

    public async Task<IActionResult> Index(string? search)
    {
        if (!await _permissions.HasPermissionAsync(User, "ConflictChecks.View"))
        {
            return Forbid();
        }

        var query = _db.ConflictChecks
            .AsNoTracking()
            .Include(x => x.ResultStatusLookup)
            .Include(x => x.Case)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.ClientName.Contains(search) || x.OpponentName.Contains(search) || (x.NationalId ?? "").Contains(search));
        }

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        ViewBag.Search = search;
        return View(items);
    }

    public async Task<IActionResult> Create(int? caseId = null)
    {
        if (!await _permissions.HasPermissionAsync(User, "ConflictChecks.Create"))
        {
            return Forbid();
        }

        var vm = new ConflictCheckVM
        {
            CaseId = caseId
        };
        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ConflictCheckVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "ConflictChecks.Create"))
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

        var statusId = await ResolveConflictStatusAsync(vm);

        _db.ConflictChecks.Add(new ConflictCheck
        {
            ClientName = vm.ClientName.Trim(),
            OpponentName = vm.OpponentName.Trim(),
            NationalId = vm.NationalId,
            Phone = vm.Phone,
            CaseId = vm.CaseId,
            ResultStatusLookupId = statusId,
            Notes = vm.Notes,
            CheckedByUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty
        });

        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تم حفظ فحص تعارض المصالح.";
        return RedirectToAction(nameof(Index));
    }

    private async Task Fill(ConflictCheckVM vm)
    {
        vm.CaseTypes = await _caseTypeOptions.GetVisibleCaseTypesAsync(User);
        vm.Cases = await BuildCasesAsync(vm.CaseTypeId);

        vm.Results = await _db.Lookups
            .Where(x => x.Type == "ConflictCheckStatus" && x.IsActive)
            .OrderBy(x => x.NameAr)
            .Select(x => new SelectListItem(x.NameAr, x.Id.ToString()))
            .ToListAsync();
    }

    private async Task<List<SelectListItem>> BuildCasesAsync(int? caseTypeId)
    {
        var query = _db.Cases.AsNoTracking().AsQueryable();
        if (caseTypeId.HasValue)
        {
            query = query.Where(x => x.CaseTypeId == caseTypeId.Value);
        }

        return await query
            .OrderByDescending(x => x.Id)
            .Select(x => new SelectListItem($"{x.CaseNumber} - {x.Title}", x.Id.ToString()))
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
            ModelState.AddModelError(nameof(ConflictCheckVM.CaseId), "القضية المختارة لا تطابق نوع القضية.");
            TempData["ToastError"] = "القضية المختارة لا تطابق نوع القضية.";
        }

        return matches;
    }

    private async Task<int> ResolveConflictStatusAsync(ConflictCheckVM vm)
    {
        if (vm.ResultStatusLookupId > 0)
        {
            return vm.ResultStatusLookupId;
        }

        var matchCount = await _db.Clients.AnyAsync(x =>
            x.FullName.Contains(vm.ClientName) ||
            x.NationalId == vm.NationalId ||
            x.Phone == vm.Phone ||
            x.WhatsAppPhone == vm.Phone)
            ? 1 : 0;

        var statusCode = matchCount > 0 ? "Possible" : "None";
        var item = await _db.Lookups.FirstOrDefaultAsync(x => x.Type == "ConflictCheckStatus" && x.NameEn == statusCode);
        return item?.Id ?? await _db.Lookups.Where(x => x.Type == "ConflictCheckStatus").Select(x => x.Id).FirstOrDefaultAsync();
    }
}
