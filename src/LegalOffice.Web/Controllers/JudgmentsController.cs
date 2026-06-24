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
public class JudgmentsController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;
    private readonly ICaseTypeOptionsService _caseTypeOptions;

    public JudgmentsController(AppDbContext db, IPermissionService permissions, ICaseTypeOptionsService caseTypeOptions)
    {
        _db = db;
        _permissions = permissions;
        _caseTypeOptions = caseTypeOptions;
    }

    public async Task<IActionResult> Index(string? search)
    {
        if (!await _permissions.HasPermissionAsync(User, "Judgments.View"))
        {
            return Forbid();
        }

        var query = _db.Judgments
            .AsNoTracking()
            .Include(x => x.Case)
            .Include(x => x.CourtLevelLookup)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Case.CaseNumber.Contains(search) ||
                (x.JudgmentSummary != null && x.JudgmentSummary.Contains(search)) ||
                x.CourtLevelLookup.NameAr.Contains(search));
        }

        ViewBag.Search = search;
        return View(await query.OrderByDescending(x => x.JudgmentDate).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        if (!await _permissions.HasPermissionAsync(User, "Judgments.Create"))
        {
            return Forbid();
        }

        var vm = new JudgmentVM { JudgmentDate = DateTime.Today };
        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JudgmentVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "Judgments.Create"))
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

        _db.Judgments.Add(new Judgment
        {
            CaseId = vm.CaseId!.Value,
            JudgmentDate = vm.JudgmentDate,
            CourtLevelLookupId = vm.CourtLevelLookupId,
            JudgmentSummary = vm.JudgmentSummary,
            JudgmentAmount = vm.JudgmentAmount,
            IsFinal = vm.IsFinal,
            Notes = vm.Notes
        });
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تم حفظ الحكم بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!await _permissions.HasPermissionAsync(User, "Judgments.Edit"))
        {
            return Forbid();
        }

        var entity = await _db.Judgments.Include(x => x.Case).FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        var vm = new JudgmentVM
        {
            Id = entity.Id,
            CaseId = entity.CaseId,
            CaseTypeId = entity.Case?.CaseTypeId,
            JudgmentDate = entity.JudgmentDate,
            CourtLevelLookupId = entity.CourtLevelLookupId,
            JudgmentSummary = entity.JudgmentSummary,
            JudgmentAmount = entity.JudgmentAmount,
            IsFinal = entity.IsFinal,
            Notes = entity.Notes
        };

        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(JudgmentVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "Judgments.Edit"))
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

        var entity = await _db.Judgments.FirstOrDefaultAsync(x => x.Id == vm.Id.Value);
        if (entity == null)
        {
            return NotFound();
        }

        entity.CaseId = vm.CaseId!.Value;
        entity.JudgmentDate = vm.JudgmentDate;
        entity.CourtLevelLookupId = vm.CourtLevelLookupId;
        entity.JudgmentSummary = vm.JudgmentSummary;
        entity.JudgmentAmount = vm.JudgmentAmount;
        entity.IsFinal = vm.IsFinal;
        entity.Notes = vm.Notes;
        await _db.SaveChangesAsync();

        TempData["ToastSuccess"] = "تم تحديث الحكم بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    private async Task Fill(JudgmentVM vm)
    {
        vm.CaseTypes = await _caseTypeOptions.GetVisibleCaseTypesAsync(User);
        vm.Cases = await BuildCasesAsync(vm.CaseTypeId);
        vm.CourtLevels = await _db.Lookups.Where(x => x.Type == "CourtLevel" && x.IsActive).OrderBy(x => x.NameAr).Select(x => new SelectListItem(x.NameAr, x.Id.ToString())).ToListAsync();
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
            ModelState.AddModelError(nameof(JudgmentVM.CaseId), "القضية المختارة لا تطابق نوع القضية.");
            TempData["ToastError"] = "القضية المختارة لا تطابق نوع القضية.";
        }

        return matches;
    }
}
