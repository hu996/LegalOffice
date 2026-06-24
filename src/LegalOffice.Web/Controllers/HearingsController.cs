using LegalOffice.Application.Interfaces;
using LegalOffice.Application.ViewModels;
using LegalOffice.Domain.Entities;
using LegalOffice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using LegalOffice.Web.Services;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class HearingsController : Controller
{
    private readonly AppDbContext _db;
    private readonly ICaseTimelineService _timeline;
    private readonly IPermissionService _permissions;
    private readonly IWorkflowStatusService _workflowStatus;

    public HearingsController(AppDbContext db, ICaseTimelineService timeline, IPermissionService permissions, IWorkflowStatusService workflowStatus)
    {
        _db = db;
        _timeline = timeline;
        _permissions = permissions;
        _workflowStatus = workflowStatus;
    }

    public async Task<IActionResult> Create(int caseId)
    {
        if (await IsCaseClosedAsync(caseId))
        {
            TempData["ToastError"] = "لا يمكن إضافة جلسات لقضية مغلقة.";
            return RedirectToAction("Details", "Cases", new { id = caseId });
        }

        if (!await CanManageCaseAsync(caseId))
        {
            return Forbid();
        }

        var vm = new HearingCreateEditVM { CaseId = caseId, HearingStatusId = await _workflowStatus.GetInitialStatusIdAsync("HearingStatus") ?? 0 };
        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HearingCreateEditVM vm)
    {
        if (await IsCaseClosedAsync(vm.CaseId))
        {
            TempData["ToastError"] = "لا يمكن إضافة جلسات لقضية مغلقة.";
            return RedirectToAction("Details", "Cases", new { id = vm.CaseId });
        }

        if (!await CanManageCaseAsync(vm.CaseId))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            await Fill(vm);
            return View(vm);
        }

        if (!await _workflowStatus.ValidateSequentialTransitionAsync("HearingStatus", null, vm.HearingStatusId, ModelState, nameof(vm.HearingStatusId), "الجلسة"))
        {
            await Fill(vm);
            return View(vm);
        }

        var hearing = new CaseHearing
        {
            CaseId = vm.CaseId,
            HearingDate = vm.HearingDate,
            HearingStatusId = vm.HearingStatusId,
            CourtDecision = vm.CourtDecision,
            Notes = vm.Notes,
            NextRequirements = vm.NextRequirements,
            NextHearingDate = vm.NextHearingDate
        };

        _db.CaseHearings.Add(hearing);
        await _db.SaveChangesAsync();
        await _timeline.AddAsync(vm.CaseId, "تمت إضافة جلسة", $"تاريخ الجلسة: {vm.HearingDate:yyyy/MM/dd}", "HearingAdded");
        TempData["ToastSuccess"] = "تمت إضافة الجلسة بنجاح.";
        return RedirectToAction("Details", "Cases", new { id = vm.CaseId });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var hearing = await _db.CaseHearings.FindAsync(id);
        if (hearing == null)
        {
            return NotFound();
        }

        if (await IsCaseClosedAsync(hearing.CaseId))
        {
            TempData["ToastError"] = "لا يمكن تعديل جلسة قضية مغلقة.";
            return RedirectToAction("Details", "Cases", new { id = hearing.CaseId });
        }

        if (!await CanManageCaseAsync(hearing.CaseId))
        {
            return Forbid();
        }

        var vm = new HearingCreateEditVM
        {
            Id = hearing.Id,
            CaseId = hearing.CaseId,
            HearingDate = hearing.HearingDate,
            HearingStatusId = hearing.HearingStatusId,
            CourtDecision = hearing.CourtDecision,
            Notes = hearing.Notes,
            NextRequirements = hearing.NextRequirements,
            NextHearingDate = hearing.NextHearingDate
        };

        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(HearingCreateEditVM vm)
    {
        if (!vm.Id.HasValue)
        {
            return BadRequest();
        }

        if (await IsCaseClosedAsync(vm.CaseId))
        {
            TempData["ToastError"] = "لا يمكن تعديل جلسة قضية مغلقة.";
            return RedirectToAction("Details", "Cases", new { id = vm.CaseId });
        }

        if (!await CanManageCaseAsync(vm.CaseId))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            await Fill(vm);
            return View(vm);
        }

        var existing = await _db.CaseHearings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == vm.Id.Value);
        if (existing == null)
        {
            return NotFound();
        }

        if (!await _workflowStatus.ValidateSequentialTransitionAsync("HearingStatus", existing.HearingStatusId, vm.HearingStatusId, ModelState, nameof(vm.HearingStatusId), "الجلسة"))
        {
            await Fill(vm);
            return View(vm);
        }

        var hearing = await _db.CaseHearings.FindAsync(vm.Id.Value);
        if (hearing == null)
        {
            return NotFound();
        }

        hearing.HearingDate = vm.HearingDate;
        hearing.HearingStatusId = vm.HearingStatusId;
        hearing.CourtDecision = vm.CourtDecision;
        hearing.Notes = vm.Notes;
        hearing.NextRequirements = vm.NextRequirements;
        hearing.NextHearingDate = vm.NextHearingDate;

        await _db.SaveChangesAsync();
        await _timeline.AddAsync(vm.CaseId, "تم تحديث جلسة", vm.CourtDecision, "HearingUpdated");
        TempData["ToastSuccess"] = "تم تحديث الجلسة بنجاح.";
        return RedirectToAction("Details", "Cases", new { id = vm.CaseId });
    }

    private async Task Fill(HearingCreateEditVM vm)
    {
        var currentStatusId = vm.Id.HasValue
            ? await _db.CaseHearings.AsNoTracking().Where(x => x.Id == vm.Id.Value).Select(x => (int?)x.HearingStatusId).FirstOrDefaultAsync()
            : null;
        vm.HearingStatuses = await _workflowStatus.GetSequentialOptionsAsync("HearingStatus", currentStatusId);
    }

    private async Task<bool> CanManageCaseAsync(int caseId)
    {
        if (await _permissions.HasPermissionAsync(User, "Cases.Edit"))
        {
            return true;
        }

        var lawyerId = await GetCurrentLawyerIdAsync();
        if (lawyerId == null)
        {
            return false;
        }

        var editId = await GetLookupIdAsync("CaseAccessLevel", "Edit");
        var manageId = await GetLookupIdAsync("CaseAccessLevel", "Manage");
        return await _db.CaseLawyers.AnyAsync(x =>
            x.CaseId == caseId &&
            x.LawyerId == lawyerId.Value &&
            x.AccessLevelId >= editId &&
            x.AccessLevelId <= manageId);
    }

    private async Task<int?> GetCurrentLawyerIdAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        return await _db.Lawyers
            .Where(x => x.UserId == userId)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync();
    }

    private async Task<int> GetLookupIdAsync(string type, string nameEn)
    {
        var item = await _db.Lookups.FirstOrDefaultAsync(x => x.Type == type && x.NameEn == nameEn);
        return item?.Id ?? 0;
    }

    private async Task<bool> IsCaseClosedAsync(int caseId)
    {
        var closedId = await _db.Lookups
            .Where(x => x.Type == "CaseStatus" && x.NameEn == "Closed")
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        return closedId > 0 && await _db.Cases.AnyAsync(x => x.Id == caseId && x.CaseStatusId == closedId);
    }
}
