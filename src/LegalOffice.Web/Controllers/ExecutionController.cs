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
public class ExecutionController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;
    private readonly ICaseTypeOptionsService _caseTypeOptions;
    private readonly IWorkflowStatusService _workflowStatus;

    public ExecutionController(AppDbContext db, IPermissionService permissions, ICaseTypeOptionsService caseTypeOptions, IWorkflowStatusService workflowStatus)
    {
        _db = db;
        _permissions = permissions;
        _caseTypeOptions = caseTypeOptions;
        _workflowStatus = workflowStatus;
    }

    public async Task<IActionResult> Index(string? search)
    {
        if (!await _permissions.HasPermissionAsync(User, "Execution.View"))
        {
            return Forbid();
        }

        var query = _db.ExecutionCases
            .AsNoTracking()
            .Include(x => x.Judgment).ThenInclude(x => x.Case)
            .Include(x => x.ExecutionStatusLookup)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Judgment.Case.CaseNumber.Contains(search) ||
                x.ExecutionStatusLookup.NameAr.Contains(search) ||
                (x.ExecutionOfficer != null && x.ExecutionOfficer.Contains(search)));
        }

        ViewBag.Search = search;
        return View(await query.OrderByDescending(x => x.CreatedAt).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        if (!await _permissions.HasPermissionAsync(User, "Execution.Create"))
        {
            return Forbid();
        }

        var vm = new ExecutionCaseVM { StartDate = DateTime.Today, ExecutionStatusLookupId = await _workflowStatus.GetInitialStatusIdAsync("ExecutionStatus") ?? 0 };
        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExecutionCaseVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "Execution.Create"))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            await Fill(vm);
            return View(vm);
        }

        if (!await _workflowStatus.ValidateSequentialTransitionAsync("ExecutionStatus", null, vm.ExecutionStatusLookupId, ModelState, nameof(vm.ExecutionStatusLookupId), "التنفيذ"))
        {
            await Fill(vm);
            return View(vm);
        }

        if (!await ValidateJudgmentTypeMatchAsync(vm.JudgmentId, vm.CaseTypeId))
        {
            await Fill(vm);
            return View(vm);
        }

        _db.ExecutionCases.Add(new ExecutionCase
        {
            JudgmentId = vm.JudgmentId!.Value,
            ExecutionStatusLookupId = vm.ExecutionStatusLookupId,
            ExecutionOfficer = vm.ExecutionOfficer,
            ExecutionNotes = vm.ExecutionNotes,
            StartDate = vm.StartDate,
            EndDate = vm.EndDate
        });
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تم إنشاء ملف التنفيذ بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!await _permissions.HasPermissionAsync(User, "Execution.Edit"))
        {
            return Forbid();
        }

        var entity = await _db.ExecutionCases
            .Include(x => x.Judgment)
            .ThenInclude(x => x.Case)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        var vm = new ExecutionCaseVM
        {
            Id = entity.Id,
            JudgmentId = entity.JudgmentId,
            CaseTypeId = entity.Judgment?.Case?.CaseTypeId,
            ExecutionStatusLookupId = entity.ExecutionStatusLookupId,
            ExecutionOfficer = entity.ExecutionOfficer,
            ExecutionNotes = entity.ExecutionNotes,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate
        };

        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ExecutionCaseVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "Execution.Edit"))
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

        if (!await ValidateJudgmentTypeMatchAsync(vm.JudgmentId, vm.CaseTypeId))
        {
            await Fill(vm);
            return View(vm);
        }

        var existing = await _db.ExecutionCases.AsNoTracking().FirstOrDefaultAsync(x => x.Id == vm.Id.Value);
        if (existing == null)
        {
            return NotFound();
        }

        if (!await _workflowStatus.ValidateSequentialTransitionAsync("ExecutionStatus", existing.ExecutionStatusLookupId, vm.ExecutionStatusLookupId, ModelState, nameof(vm.ExecutionStatusLookupId), "التنفيذ"))
        {
            await Fill(vm);
            return View(vm);
        }

        var entity = await _db.ExecutionCases.FirstOrDefaultAsync(x => x.Id == vm.Id.Value);
        if (entity == null)
        {
            return NotFound();
        }

        entity.JudgmentId = vm.JudgmentId!.Value;
        entity.ExecutionStatusLookupId = vm.ExecutionStatusLookupId;
        entity.ExecutionOfficer = vm.ExecutionOfficer;
        entity.ExecutionNotes = vm.ExecutionNotes;
        entity.StartDate = vm.StartDate;
        entity.EndDate = vm.EndDate;
        await _db.SaveChangesAsync();

        TempData["ToastSuccess"] = "تم تحديث ملف التنفيذ بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    private async Task Fill(ExecutionCaseVM vm)
    {
        vm.CaseTypes = await _caseTypeOptions.GetVisibleCaseTypesAsync(User);
        vm.Judgments = await BuildJudgmentsAsync(vm.CaseTypeId);
        var currentStatusId = vm.Id.HasValue
            ? await _db.ExecutionCases.AsNoTracking().Where(x => x.Id == vm.Id.Value).Select(x => (int?)x.ExecutionStatusLookupId).FirstOrDefaultAsync()
            : null;
        vm.Statuses = await _workflowStatus.GetSequentialOptionsAsync("ExecutionStatus", currentStatusId);
    }

    private async Task<List<SelectListItem>> BuildJudgmentsAsync(int? caseTypeId)
    {
        var query = _db.Judgments.AsNoTracking()
            .Include(x => x.Case)
            .AsQueryable();

        if (caseTypeId.HasValue)
        {
            query = query.Where(x => x.Case.CaseTypeId == caseTypeId.Value);
        }

        return await query
            .OrderByDescending(x => x.JudgmentDate)
            .Select(x => new SelectListItem($"{x.Case.CaseNumber} - {x.JudgmentDate:yyyy/MM/dd}", x.Id.ToString()))
            .ToListAsync();
    }

    [HttpGet]
    public async Task<IActionResult> EligibleJudgments(int? caseTypeId)
    {
        var items = await BuildJudgmentsAsync(caseTypeId);
        return Json(items.Select(x => new { value = x.Value, text = x.Text }));
    }

    private async Task<bool> ValidateJudgmentTypeMatchAsync(int? judgmentId, int? caseTypeId)
    {
        if (!judgmentId.HasValue || !caseTypeId.HasValue)
        {
            return true;
        }

        var matches = await _db.Judgments.AnyAsync(x => x.Id == judgmentId.Value && x.Case.CaseTypeId == caseTypeId.Value);
        if (!matches)
        {
            ModelState.AddModelError(nameof(ExecutionCaseVM.JudgmentId), "الحكم المختار لا يطابق نوع القضية.");
            TempData["ToastError"] = "الحكم المختار لا يطابق نوع القضية.";
        }

        return matches;
    }
}
