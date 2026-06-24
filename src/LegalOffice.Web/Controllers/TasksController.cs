using LegalOffice.Application.Interfaces;
using LegalOffice.Application.ViewModels;
using LegalOffice.Domain.Entities;
using LegalOffice.Infrastructure.Persistence;
using LegalOffice.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class TasksController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;
    private readonly ICaseTimelineService _timeline;
    private readonly ICaseTypeOptionsService _caseTypeOptions;
    private readonly IWorkflowStatusService _workflowStatus;

    public TasksController(AppDbContext db, IPermissionService permissions, ICaseTimelineService timeline, ICaseTypeOptionsService caseTypeOptions, IWorkflowStatusService workflowStatus)
    {
        _db = db;
        _permissions = permissions;
        _timeline = timeline;
        _caseTypeOptions = caseTypeOptions;
        _workflowStatus = workflowStatus;
    }

    public async Task<IActionResult> Index(int? statusId, int? priorityId, string? assigneeId, int? caseId, DateTime? dueDate)
    {
        return await RenderListAsync(statusId, priorityId, assigneeId, caseId, dueDate);
    }

    public async Task<IActionResult> Mine()
    {
        return await RenderListAsync(null, null, CurrentUserId(), null, null);
    }

    public async Task<IActionResult> Today()
    {
        if (!await _permissions.HasPermissionAsync(User, "Tasks.ViewToday")
            && !await _permissions.HasPermissionAsync(User, "Tasks.View")
            && !await _permissions.HasPermissionAsync(User, "Tasks.ViewMine"))
        {
            return Forbid();
        }

        return await RenderListAsync(null, null, CurrentUserId(), null, DateTime.Today);
    }

    public async Task<IActionResult> Late()
    {
        if (!await _permissions.HasPermissionAsync(User, "Tasks.ViewLate")
            && !await _permissions.HasPermissionAsync(User, "Tasks.View")
            && !await _permissions.HasPermissionAsync(User, "Tasks.ViewMine"))
        {
            return Forbid();
        }

        var today = DateTime.Today;
        var query = VisibleTasksQuery(await CanViewAllAsync(), CurrentUserId()).Where(x => !x.IsDeleted && x.CompletedAt == null && x.DueDate < today);
        var items = await query
            .OrderBy(x => x.DueDate)
            .Select(x => new LegalTaskListItemVM
            {
                Id = x.Id,
                Title = x.Title,
                RelatedCaseNumber = x.RelatedCase != null ? x.RelatedCase.CaseNumber : null,
                AssignedToUserName = x.AssignedToUser.FullName,
                StatusName = x.StatusLookup.NameAr,
                PriorityName = x.PriorityLookup.NameAr,
                TaskTypeName = x.TaskTypeLookup.NameAr,
                DueDate = x.DueDate,
                IsDeleted = x.IsDeleted,
                CompletedAt = x.CompletedAt
            })
            .ToListAsync();

        await FillFilters();
        ViewBag.DueDate = null;
        return View("Index", items);
    }

    public async Task<IActionResult> Create(int? caseId = null)
    {
        if (!await _permissions.HasPermissionAsync(User, "Tasks.Create"))
        {
            return Forbid();
        }

        if (caseId.HasValue && await IsCaseClosedAsync(caseId.Value))
        {
            TempData["ToastError"] = "لا يمكن إضافة مهمة على قضية مغلقة.";
            return RedirectToAction("Details", "Cases", new { id = caseId.Value });
        }

        var vm = new LegalTaskEditVM
        {
            RelatedCaseId = caseId,
            DueDate = DateTime.Today,
            StatusLookupId = await _workflowStatus.GetInitialStatusIdAsync("TaskStatus") ?? 0
        };
        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LegalTaskEditVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "Tasks.Create"))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            await Fill(vm);
            return View(vm);
        }

        if (!await _workflowStatus.ValidateSequentialTransitionAsync("TaskStatus", null, vm.StatusLookupId, ModelState, nameof(vm.StatusLookupId), "المهمة"))
        {
            await Fill(vm);
            return View(vm);
        }

        if (!await ValidateCaseTypeMatchAsync(vm.RelatedCaseId, vm.CaseTypeId))
        {
            await Fill(vm);
            return View(vm);
        }

        if (vm.RelatedCaseId.HasValue && await IsCaseClosedAsync(vm.RelatedCaseId.Value))
        {
            TempData["ToastError"] = "لا يمكن إضافة مهمة على قضية مغلقة.";
            await Fill(vm);
            return View(vm);
        }

        var task = new LegalTask
        {
            Title = vm.Title.Trim(),
            Description = vm.Description,
            RelatedCaseId = vm.RelatedCaseId,
            AssignedToUserId = vm.AssignedToUserId,
            CreatedByUserId = CurrentUserId()!,
            DueDate = vm.DueDate,
            PriorityLookupId = vm.PriorityLookupId,
            StatusLookupId = vm.StatusLookupId,
            TaskTypeLookupId = vm.TaskTypeLookupId
        };

        _db.LegalTasks.Add(task);
        await _db.SaveChangesAsync();

        if (vm.RelatedCaseId.HasValue)
        {
            await _timeline.AddAsync(vm.RelatedCaseId.Value, "تمت إضافة مهمة", vm.Title, "TaskCreated");
        }

        TempData["ToastSuccess"] = "تمت إضافة المهمة بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!await _permissions.HasPermissionAsync(User, "Tasks.Edit"))
        {
            return Forbid();
        }

        var task = await _db.LegalTasks.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (task == null)
        {
            return NotFound();
        }

        var vm = new LegalTaskEditVM
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            RelatedCaseId = task.RelatedCaseId,
            AssignedToUserId = task.AssignedToUserId,
            DueDate = task.DueDate,
            PriorityLookupId = task.PriorityLookupId,
            StatusLookupId = task.StatusLookupId,
            TaskTypeLookupId = task.TaskTypeLookupId
        };

        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(LegalTaskEditVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "Tasks.Edit"))
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

        if (!await ValidateCaseTypeMatchAsync(vm.RelatedCaseId, vm.CaseTypeId))
        {
            await Fill(vm);
            return View(vm);
        }

        if (vm.RelatedCaseId.HasValue && await IsCaseClosedAsync(vm.RelatedCaseId.Value))
        {
            TempData["ToastError"] = "لا يمكن تعديل مهمة مرتبطة بقضية مغلقة.";
            await Fill(vm);
            return View(vm);
        }

        var existing = await _db.LegalTasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == vm.Id.Value && !x.IsDeleted);
        if (existing == null)
        {
            return NotFound();
        }

        if (!await _workflowStatus.ValidateSequentialTransitionAsync("TaskStatus", existing.StatusLookupId, vm.StatusLookupId, ModelState, nameof(vm.StatusLookupId), "المهمة"))
        {
            await Fill(vm);
            return View(vm);
        }

        var task = await _db.LegalTasks.FirstOrDefaultAsync(x => x.Id == vm.Id.Value && !x.IsDeleted);
        if (task == null)
        {
            return NotFound();
        }

        task.Title = vm.Title.Trim();
        task.Description = vm.Description;
        task.RelatedCaseId = vm.RelatedCaseId;
        task.AssignedToUserId = vm.AssignedToUserId;
        task.DueDate = vm.DueDate;
        task.PriorityLookupId = vm.PriorityLookupId;
        task.StatusLookupId = vm.StatusLookupId;
        task.TaskTypeLookupId = vm.TaskTypeLookupId;
        task.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();

        TempData["ToastSuccess"] = "تم تعديل المهمة بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id, int statusId)
    {
        if (!await _permissions.HasPermissionAsync(User, "Tasks.ChangeStatus"))
        {
            return Forbid();
        }

        var task = await _db.LegalTasks.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (task == null)
        {
            return NotFound();
        }

        if (task.RelatedCaseId.HasValue && await IsCaseClosedAsync(task.RelatedCaseId.Value))
        {
            TempData["ToastError"] = "لا يمكن تعديل مهمة مرتبطة بقضية مغلقة.";
            return RedirectToAction(nameof(Index));
        }

        task.StatusLookupId = statusId;
        task.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();

        TempData["ToastSuccess"] = "تم تغيير حالة المهمة.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(int id)
    {
        if (!await _permissions.HasPermissionAsync(User, "Tasks.Close"))
        {
            return Forbid();
        }

        var task = await _db.LegalTasks.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (task == null)
        {
            return NotFound();
        }

        if (task.RelatedCaseId.HasValue && await IsCaseClosedAsync(task.RelatedCaseId.Value))
        {
            TempData["ToastError"] = "لا يمكن إغلاق مهمة مرتبطة بقضية مغلقة.";
            return RedirectToAction(nameof(Index));
        }

        task.CompletedAt = DateTime.Now;
        task.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تم إغلاق المهمة.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _permissions.HasPermissionAsync(User, "Tasks.Delete"))
        {
            return Forbid();
        }

        var task = await _db.LegalTasks.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (task == null)
        {
            return NotFound();
        }

        if (task.RelatedCaseId.HasValue && await IsCaseClosedAsync(task.RelatedCaseId.Value))
        {
            TempData["ToastError"] = "لا يمكن حذف مهمة مرتبطة بقضية مغلقة.";
            return RedirectToAction(nameof(Index));
        }

        task.IsDeleted = true;
        task.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تم حذف المهمة بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    private IQueryable<LegalTask> VisibleTasksQuery(bool canViewAll, string? userId)
    {
        if (canViewAll)
        {
            return _db.LegalTasks
                .Include(x => x.RelatedCase)
                .Include(x => x.AssignedToUser)
                .Include(x => x.PriorityLookup)
                .Include(x => x.StatusLookup)
                .Include(x => x.TaskTypeLookup)
                .Where(x => !x.IsDeleted);
        }

        return _db.LegalTasks
            .Include(x => x.RelatedCase)
            .Include(x => x.AssignedToUser)
            .Include(x => x.PriorityLookup)
            .Include(x => x.StatusLookup)
            .Include(x => x.TaskTypeLookup)
            .Where(x => !x.IsDeleted && x.AssignedToUserId == userId);
    }

    private async Task<bool> CanViewAllAsync()
    {
        return await _permissions.HasPermissionAsync(User, "Tasks.View")
            || await _permissions.HasPermissionAsync(User, "Tasks.ViewLate")
            || await _permissions.HasPermissionAsync(User, "Tasks.ViewToday");
    }

    private async Task FillFilters()
    {
        ViewBag.Statuses = await SelectLookups("TaskStatus");
        ViewBag.Priorities = await SelectLookups("TaskPriority");
        ViewBag.Types = await SelectLookups("TaskType");
        ViewBag.Users = await _db.Users
            .OrderBy(x => x.FullName)
            .Select(x => new SelectListItem(x.FullName, x.Id.ToString()))
            .ToListAsync();
        ViewBag.Cases = await BuildCasesAsync(null);
    }

    private async Task Fill(LegalTaskEditVM vm)
    {
        await FillFilters();
        var currentStatusId = vm.Id.HasValue
            ? await _db.LegalTasks.AsNoTracking().Where(x => x.Id == vm.Id.Value && !x.IsDeleted).Select(x => (int?)x.StatusLookupId).FirstOrDefaultAsync()
            : null;
        vm.Statuses = await _workflowStatus.GetSequentialOptionsAsync("TaskStatus", currentStatusId);
        vm.Priorities = (IEnumerable<SelectListItem>)ViewBag.Priorities;
        vm.Types = (IEnumerable<SelectListItem>)ViewBag.Types;
        vm.Assignees = (IEnumerable<SelectListItem>)ViewBag.Users;
        vm.CaseTypes = await SelectLookups("CaseType");
        vm.Cases = await BuildCasesAsync(vm.CaseTypeId);
    }

    private async Task<IActionResult> RenderListAsync(int? statusId, int? priorityId, string? assigneeId, int? caseId, DateTime? dueDate)
    {
        var canViewAll = await CanViewAllAsync();
        var canViewMine = await _permissions.HasPermissionAsync(User, "Tasks.ViewMine");
        if (!canViewAll && !canViewMine)
        {
            return Forbid();
        }

        var query = VisibleTasksQuery(canViewAll, CurrentUserId());
        if (statusId.HasValue) query = query.Where(x => x.StatusLookupId == statusId);
        if (priorityId.HasValue) query = query.Where(x => x.PriorityLookupId == priorityId);
        if (!string.IsNullOrWhiteSpace(assigneeId)) query = query.Where(x => x.AssignedToUserId == assigneeId);
        if (caseId.HasValue) query = query.Where(x => x.RelatedCaseId == caseId);
        if (dueDate.HasValue) query = query.Where(x => x.DueDate.Date == dueDate.Value.Date);

        ViewBag.StatusId = statusId;
        ViewBag.PriorityId = priorityId;
        ViewBag.AssigneeId = assigneeId;
        ViewBag.CaseId = caseId;
        ViewBag.DueDate = dueDate?.ToString("yyyy-MM-dd");
        await FillFilters();

        var items = await query
            .OrderBy(x => x.IsDeleted)
            .ThenBy(x => x.DueDate)
            .Select(x => new LegalTaskListItemVM
            {
                Id = x.Id,
                Title = x.Title,
                RelatedCaseNumber = x.RelatedCase != null ? x.RelatedCase.CaseNumber : null,
                AssignedToUserName = x.AssignedToUser.FullName,
                StatusName = x.StatusLookup.NameAr,
                PriorityName = x.PriorityLookup.NameAr,
                TaskTypeName = x.TaskTypeLookup.NameAr,
                DueDate = x.DueDate,
                IsDeleted = x.IsDeleted,
                CompletedAt = x.CompletedAt
            })
            .ToListAsync();

        return View("Index", items);
    }

    private string? CurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

    private async Task<List<SelectListItem>> SelectLookups(string type)
    {
        if (type == "CaseType")
        {
            return await _caseTypeOptions.GetVisibleCaseTypesAsync(User);
        }

        return await _db.Lookups
            .Where(x => x.Type == type && x.IsActive)
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
            ModelState.AddModelError(nameof(LegalTaskEditVM.RelatedCaseId), "القضية المختارة لا تطابق نوع القضية.");
            TempData["ToastError"] = "القضية المختارة لا تطابق نوع القضية.";
        }

        return matches;
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
