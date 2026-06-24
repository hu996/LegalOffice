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
public class CasesController : Controller
{
    private const string LawyerSelectPlaceholder = "اختر المحامي";

    private readonly AppDbContext _db;
    private readonly ICaseTimelineService _timeline;
    private readonly IPermissionService _permissions;
    private readonly ICaseTypeOptionsService _caseTypeOptions;
    private readonly IWorkflowStatusService _workflowStatus;

    public CasesController(AppDbContext db, ICaseTimelineService timeline, IPermissionService permissions, ICaseTypeOptionsService caseTypeOptions, IWorkflowStatusService workflowStatus)
    {
        _db = db;
        _timeline = timeline;
        _permissions = permissions;
        _caseTypeOptions = caseTypeOptions;
        _workflowStatus = workflowStatus;
    }

    public async Task<IActionResult> Index(string? search, int? statusId, int? caseTypeId, int page = 1, int pageSize = 10)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 50);

        var query = BuildVisibleCasesQuery();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.CaseNumber.Contains(search) ||
                x.Title.Contains(search) ||
                x.Client.FullName.Contains(search));
        }

        if (statusId.HasValue)
        {
            query = query.Where(x => x.CaseStatusId == statusId.Value);
        }

        if (caseTypeId.HasValue)
        {
            query = query.Where(x => x.CaseTypeId == caseTypeId.Value);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Statuses = await SelectLookups("CaseStatus");
        ViewBag.CaseTypes = await SelectLookups("CaseType");
        ViewBag.Search = search;
        ViewBag.StatusId = statusId;
        ViewBag.CaseTypeId = caseTypeId;

        return View(new PagedResult<LegalCase>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await LoadCaseDetailsAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        ViewBag.Statuses = await SelectLookups("CaseStatus");
        SetCaseSummaryViewBag(item);
        ViewBag.CanViewFees = await CanCurrentUserViewFeesAsync(item);
        ViewBag.CanManageCaseRelations = await CanCurrentUserManageCaseRelationsAsync(item.Id);
        ViewBag.CanManagePayments = await _permissions.HasPermissionAsync(User, "Payments.Create");
        return View(item);
    }

    public async Task<IActionResult> Create()
    {
        var vm = new CaseCreateEditVM
        {
            LawyersCount = 1,
            PriorityId = await GetDefaultLookupIdAsync("CasePriority", "Medium"),
            CaseYear = DateTime.Today.Year,
            CaseStatusId = await _workflowStatus.GetInitialStatusIdAsync("CaseStatus") ?? 0
        };

        InitializeLawyerAssignments(vm);
        await FillLookups(vm);
        ViewBag.CanViewFees = true;
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CaseCreateEditVM vm)
    {
        NormalizeAssignments(vm);
        ValidateAssignments(vm);
        await ValidateCaseLawyerSpecialtiesAsync(vm);
        await _workflowStatus.ValidateSequentialTransitionAsync(
            "CaseStatus",
            null,
            vm.CaseStatusId,
            ModelState,
            nameof(vm.CaseStatusId),
            "القضية");
        await ValidateUniqueCaseNumberAsync(vm);

        if (!ModelState.IsValid)
        {
            await FillLookups(vm);
            ViewBag.CanViewFees = true;
            return View(vm);
        }

        var entity = new LegalCase
        {
            CaseNumber = vm.CaseNumber,
            Title = vm.Title,
            Description = vm.Description,
            ClientId = vm.ClientId,
            CaseTypeId = vm.CaseTypeId,
            CaseStatusId = vm.CaseStatusId,
            CourtId = vm.CourtId,
            Circuit = vm.Circuit,
            OpponentName = vm.OpponentName,
            OpponentLawyer = vm.OpponentLawyer,
            StartDate = vm.StartDate,
            FeesAmount = vm.FeesAmount,
            CaseYear = vm.CaseYear,
            LawyersCount = vm.LawyerAssignments.Count,
            PriorityId = vm.PriorityId
        };

        entity.CreatedByUserId = CurrentUserId();

        foreach (var assignment in vm.LawyerAssignments)
        {
            entity.CaseLawyers.Add(new CaseLawyer
            {
                LawyerId = assignment.LawyerId,
                AccessLevelId = assignment.AccessLevelId,
                IsMainLawyer = entity.CaseLawyers.Count == 0,
                RoleInCase = assignment.AccessLevelId == await GetLookupIdAsync("CaseAccessLevel", "Manage") ? "Lead" : "Assigned"
            });
        }

        _db.Cases.Add(entity);
        await _db.SaveChangesAsync();
        try
        {
            await CreateAssignmentNotificationsAsync(entity.Id, entity.CaseNumber, entity.Title, vm.LawyerAssignments.Select(x => x.LawyerId).ToList());
        }
        catch
        {
            TempData["ToastError"] = "تم حفظ القضية لكن فشل إرسال بعض الإشعارات.";
        }

        await _timeline.AddAsync(entity.Id, "تم إنشاء القضية", $"رقم القضية: {entity.CaseNumber}", "CaseCreated");
        TempData["ToastSuccess"] = "تم حفظ القضية بنجاح.";
        return RedirectToAction(nameof(Details), new { id = entity.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _db.Cases
            .Include(x => x.CaseLawyers)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
        {
            return NotFound();
        }

        if (await IsClosedCaseAsync(entity.Id))
        {
            TempData["ToastError"] = "لا يمكن تعديل قضية مغلقة.";
            return RedirectToAction(nameof(Details), new { id = entity.Id });
        }

        var vm = new CaseCreateEditVM
        {
            Id = entity.Id,
            CaseNumber = entity.CaseNumber,
            Title = entity.Title,
            Description = entity.Description,
            ClientId = entity.ClientId,
            CaseTypeId = entity.CaseTypeId,
            CaseStatusId = entity.CaseStatusId,
            CourtId = entity.CourtId,
            Circuit = entity.Circuit,
            OpponentName = entity.OpponentName,
            OpponentLawyer = entity.OpponentLawyer,
            StartDate = entity.StartDate,
            FeesAmount = entity.FeesAmount,
            CaseYear = entity.CaseYear,
            LawyersCount = Math.Max(entity.CaseLawyers.Count, 1),
            PriorityId = entity.PriorityId,
            LawyerAssignments = entity.CaseLawyers
                .Select(x => new CaseLawyerAssignmentVM
                {
                    LawyerId = x.LawyerId,
                    AccessLevelId = x.AccessLevelId
                })
                .ToList()
        };

        InitializeLawyerAssignments(vm);
        await FillLookups(vm);
        ViewBag.CanViewFees = await CanCurrentUserViewFeesForEditAsync(entity);
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CaseCreateEditVM vm)
    {
        if (!vm.Id.HasValue)
        {
            return BadRequest();
        }

        if (await IsClosedCaseAsync(vm.Id.Value))
        {
            TempData["ToastError"] = "لا يمكن تعديل قضية مغلقة.";
            return RedirectToAction(nameof(Details), new { id = vm.Id.Value });
        }

        NormalizeAssignments(vm);
        ValidateAssignments(vm);
        await ValidateCaseLawyerSpecialtiesAsync(vm);
        await ValidateUniqueCaseNumberAsync(vm);

        if (!ModelState.IsValid)
        {
            await FillLookups(vm);
            var entityForFees = await _db.Cases.AsNoTracking().FirstOrDefaultAsync(x => x.Id == vm.Id.Value);
            ViewBag.CanViewFees = entityForFees != null && await CanCurrentUserViewFeesForEditAsync(entityForFees);
            return View(vm);
        }

        var entity = await _db.Cases.Include(x => x.CaseLawyers).FirstOrDefaultAsync(x => x.Id == vm.Id.Value);
        if (entity == null)
        {
            return NotFound();
        }

        if (!await _workflowStatus.ValidateSequentialTransitionAsync(
                "CaseStatus",
                entity.CaseStatusId,
                vm.CaseStatusId,
                ModelState,
                nameof(vm.CaseStatusId),
                "القضية"))
        {
            await FillLookups(vm);
            ViewBag.CanViewFees = await CanCurrentUserViewFeesForEditAsync(entity);
            return View(vm);
        }

        var canViewFees = await CanCurrentUserViewFeesForEditAsync(entity);
        if (!canViewFees)
        {
            vm.FeesAmount = entity.FeesAmount;
        }

        var previousLawyerIds = entity.CaseLawyers.Select(x => x.LawyerId).ToList();

        entity.CaseNumber = vm.CaseNumber;
        entity.Title = vm.Title;
        entity.Description = vm.Description;
        entity.ClientId = vm.ClientId;
        entity.CaseTypeId = vm.CaseTypeId;
        entity.CaseStatusId = vm.CaseStatusId;
        entity.CourtId = vm.CourtId;
        entity.Circuit = vm.Circuit;
        entity.OpponentName = vm.OpponentName;
        entity.OpponentLawyer = vm.OpponentLawyer;
        entity.StartDate = vm.StartDate;
        entity.FeesAmount = vm.FeesAmount;
        entity.CaseYear = vm.CaseYear;
        entity.PriorityId = vm.PriorityId;
        entity.LawyersCount = vm.LawyerAssignments.Count;

        _db.CaseLawyers.RemoveRange(entity.CaseLawyers);
        entity.CaseLawyers.Clear();

        foreach (var assignment in vm.LawyerAssignments)
        {
            entity.CaseLawyers.Add(new CaseLawyer
            {
                LawyerId = assignment.LawyerId,
                AccessLevelId = assignment.AccessLevelId,
                IsMainLawyer = entity.CaseLawyers.Count == 0,
                RoleInCase = assignment.AccessLevelId == await GetLookupIdAsync("CaseAccessLevel", "Manage") ? "Lead" : "Assigned"
            });
        }

        await _db.SaveChangesAsync();
        try
        {
            await CreateAssignmentNotificationsAsync(entity.Id, entity.CaseNumber, entity.Title, vm.LawyerAssignments.Select(x => x.LawyerId).ToList().Except(previousLawyerIds).ToList());
        }
        catch
        {
            TempData["ToastError"] = "تم تحديث القضية لكن فشل إرسال بعض الإشعارات.";
        }
        await _timeline.AddAsync(entity.Id, "تم تعديل بيانات القضية", null, "CaseUpdated");
        TempData["ToastSuccess"] = "تم تحديث بيانات القضية بنجاح.";
        return RedirectToAction(nameof(Details), new { id = entity.Id });
    }

    [HttpPost]
    public async Task<IActionResult> ChangeStatus(int id, int statusId)
    {
        var entity = await _db.Cases.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        if (await IsClosedCaseAsync(id))
        {
            TempData["ToastError"] = "لا يمكن تغيير حالة قضية مغلقة.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (!await _workflowStatus.ValidateSequentialTransitionAsync(
                "CaseStatus",
                entity.CaseStatusId,
                statusId,
                ModelState,
                "statusId",
                "القضية"))
        {
            TempData["ToastError"] = "لا يمكن تخطي تسلسل حالات القضية.";
            return RedirectToAction(nameof(Details), new { id });
        }

        entity.CaseStatusId = statusId;
        await _db.SaveChangesAsync();
        await _timeline.AddAsync(id, "تم تغيير حالة القضية", $"الحالة الجديدة رقم: {statusId}", "StatusChanged");
        TempData["ToastSuccess"] = "تم تغيير حالة القضية بنجاح.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> EligibleLawyers(int? caseTypeId)
    {
        var query = _db.Lawyers
            .Where(x => x.IsActive)
            .AsQueryable();

        if (caseTypeId.HasValue)
        {
            query = query.Where(x => x.Specialties.Any(s => s.CaseTypeId == caseTypeId.Value));
        }

        var lawyers = await query
            .OrderBy(x => x.FullName)
            .Select(x => new
            {
                value = x.Id,
                text = x.FullName
            })
            .ToListAsync();

        return Json(lawyers);
    }

    [HttpGet]
    public async Task<IActionResult> EligibleCases(int? caseTypeId)
    {
        var query = _db.Cases
            .AsNoTracking()
            .Include(x => x.Client)
            .AsQueryable();

        if (caseTypeId.HasValue)
        {
            query = query.Where(x => x.CaseTypeId == caseTypeId.Value);
        }

        var cases = await query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new
            {
                value = x.Id,
                text = x.CaseNumber + " - " + x.Client.FullName
            })
            .ToListAsync();

        return Json(cases);
    }

    private IQueryable<LegalCase> BuildVisibleCasesQuery()
    {
        var query = _db.Cases
            .Include(x => x.Client)
            .Include(x => x.CaseType)
            .Include(x => x.CaseStatus)
            .Include(x => x.CaseLawyers).ThenInclude(x => x.Lawyer)
            .AsQueryable();

        if (HasCaseEditAccess())
        {
            return query;
        }

        var lawyerId = GetCurrentLawyerId();
        if (lawyerId == null)
        {
            return query.Where(x => false);
        }

        return query.Where(x => x.CaseLawyers.Any(cl => cl.LawyerId == lawyerId.Value));
    }

    private async Task<LegalCase?> LoadCaseDetailsAsync(int id)
    {
        var query = _db.Cases
            .Include(x => x.Client)
            .Include(x => x.CaseType)
            .Include(x => x.CaseStatus)
            .Include(x => x.Court)
            .Include(x => x.Priority)
            .Include(x => x.CaseLawyers).ThenInclude(x => x.Lawyer)
            .Include(x => x.CaseLawyers).ThenInclude(x => x.AccessLevel)
            .Include(x => x.Hearings).ThenInclude(x => x.HearingStatus)
            .Include(x => x.Documents).ThenInclude(x => x.DocumentType)
            .Include(x => x.Timelines)
            .Include(x => x.Payments).ThenInclude(x => x.PaymentStatus)
            .Include(x => x.Payments).ThenInclude(x => x.PaymentMethod)
            .AsQueryable();

        if (!HasCaseEditAccess())
        {
            var lawyerId = GetCurrentLawyerId();
            if (lawyerId == null)
            {
                return null;
            }

            query = query.Where(x => x.CaseLawyers.Any(cl => cl.LawyerId == lawyerId.Value));
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    private void SetCaseSummaryViewBag(LegalCase item)
    {
        var paid = item.Payments
            .Where(x => string.Equals(x.PaymentStatus?.NameEn, "Received", StringComparison.OrdinalIgnoreCase))
            .Sum(x => x.Amount);

        ViewBag.TotalPaid = paid;
        ViewBag.Remaining = Math.Max(item.FeesAmount - paid, 0);
    }

    private string? CurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

    private int? GetCurrentLawyerId()
    {
        var userId = CurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        return _db.Lawyers
            .Where(x => x.UserId == userId)
            .Select(x => (int?)x.Id)
            .FirstOrDefault();
    }

    private async Task<bool> CanCurrentUserManageCaseRelationsAsync(int caseId)
    {
        if (await _permissions.HasPermissionAsync(User, "Cases.Edit"))
        {
            return true;
        }

        var lawyerId = GetCurrentLawyerId();
        if (lawyerId == null)
        {
            return false;
        }

        var manageId = await GetLookupIdAsync("CaseAccessLevel", "Manage");
        return await _db.CaseLawyers.AnyAsync(x => x.CaseId == caseId && x.LawyerId == lawyerId.Value && x.AccessLevelId >= manageId);
    }

    private async Task<bool> CanCurrentUserViewFeesAsync(LegalCase item)
    {
        if (await _permissions.HasPermissionAsync(User, "Treasury.View"))
        {
            return true;
        }

        var userId = CurrentUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return false;
        }

        if (string.Equals(item.CreatedByUserId, userId, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var departmentCode = await _db.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Department != null ? x.Department.NameEn : null)
            .FirstOrDefaultAsync();

        return string.Equals(departmentCode, "Finance", StringComparison.OrdinalIgnoreCase);
    }

    private async Task<bool> CanCurrentUserViewFeesForEditAsync(LegalCase item)
    {
        if (await CanCurrentUserViewFeesAsync(item))
        {
            return true;
        }

        var userId = CurrentUserId();
        return !string.IsNullOrWhiteSpace(userId) &&
               string.Equals(item.CreatedByUserId, userId, StringComparison.OrdinalIgnoreCase);
    }

    private void InitializeLawyerAssignments(CaseCreateEditVM vm)
    {
        if (vm.LawyerAssignments.Count == 0)
        {
            for (var i = 0; i < Math.Max(1, vm.LawyersCount); i++)
            {
                vm.LawyerAssignments.Add(new CaseLawyerAssignmentVM());
            }
        }
    }

    private void NormalizeAssignments(CaseCreateEditVM vm)
    {
        vm.LawyerAssignments ??= new List<CaseLawyerAssignmentVM>();
        vm.LawyerAssignments = vm.LawyerAssignments
            .Where(x => x.LawyerId > 0 && x.AccessLevelId > 0)
            .ToList();
    }

    private void ValidateAssignments(CaseCreateEditVM vm)
    {
        if (vm.LawyerAssignments.Count != vm.LawyersCount)
        {
            ModelState.AddModelError(nameof(vm.LawyerAssignments), $"لازم تختار {vm.LawyersCount} محامي بالظبط.");
            TempData["ToastError"] = $"لازم تختار {vm.LawyersCount} محامي بالظبط.";
        }

        var lawyerIds = vm.LawyerAssignments.Select(x => x.LawyerId).ToList();
        if (lawyerIds.Count != lawyerIds.Distinct().Count())
        {
            ModelState.AddModelError(nameof(vm.LawyerAssignments), "كل محامي لازم يكون مختلف عن الباقي.");
            TempData["ToastError"] = "كل محامي لازم يكون مختلف عن الباقي.";
        }
    }

    private async Task FillLookups(CaseCreateEditVM vm)
    {
        vm.Clients = await _db.Clients
            .OrderBy(x => x.FullName)
            .Select(x => new SelectListItem(x.FullName, x.Id.ToString()))
            .ToListAsync();

        vm.CaseTypes = await SelectLookups("CaseType");
        vm.Courts = await SelectLookups("Court");
        vm.AccessLevels = await SelectLookups("CaseAccessLevel");
        vm.Priorities = await SelectLookups("CasePriority");
        var currentStatusId = vm.Id.HasValue
            ? await _db.Cases.AsNoTracking().Where(x => x.Id == vm.Id.Value).Select(x => (int?)x.CaseStatusId).FirstOrDefaultAsync()
            : null;
        vm.CaseStatuses = await _workflowStatus.GetSequentialOptionsAsync("CaseStatus", currentStatusId);
    }

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

    private async Task<int> GetLookupIdAsync(string type, string nameEn)
    {
        var item = await _db.Lookups.FirstOrDefaultAsync(x => x.Type == type && x.NameEn == nameEn);
        return item?.Id ?? 0;
    }

    private async Task<int> GetDefaultLookupIdAsync(string type, string nameEn)
    {
        var item = await _db.Lookups.FirstOrDefaultAsync(x => x.Type == type && x.NameEn == nameEn);
        if (item != null)
        {
            return item.Id;
        }

        return await _db.Lookups
            .Where(x => x.Type == type && x.IsActive)
            .OrderBy(x => x.NameAr)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();
    }

    private async Task ValidateUniqueCaseNumberAsync(CaseCreateEditVM vm)
    {
        if (string.IsNullOrWhiteSpace(vm.CaseNumber))
        {
            return;
        }

        var year = vm.CaseYear;
        var exists = await _db.Cases.AnyAsync(x =>
            x.CaseNumber == vm.CaseNumber &&
            x.CaseTypeId == vm.CaseTypeId &&
            x.CaseYear == year &&
            x.Id != (vm.Id ?? 0));
        if (exists)
        {
            ModelState.AddModelError(nameof(vm.CaseNumber), "رقم القضية موجود بالفعل لنفس النوع والسنة.");
            TempData["ToastError"] = "رقم القضية موجود بالفعل لنفس النوع والسنة.";
        }
    }

    private bool HasCaseEditAccess()
    {
        return _permissions.HasPermissionAsync(User, "Cases.Edit").GetAwaiter().GetResult();
    }

    private async Task<bool> IsClosedCaseAsync(int caseId)
    {
        var closedId = await _db.Lookups
            .Where(x => x.Type == "CaseStatus" && x.NameEn == "Closed")
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        return closedId > 0 && await _db.Cases.AnyAsync(x => x.Id == caseId && x.CaseStatusId == closedId);
    }

    private async Task CreateAssignmentNotificationsAsync(int caseId, string caseNumber, string caseTitle, List<int> lawyerIds)
    {
        if (lawyerIds.Count == 0)
        {
            return;
        }

        var assignedLawyers = await _db.Lawyers
            .Where(x => lawyerIds.Contains(x.Id) && x.UserId != null)
            .Select(x => new
            {
                x.Id,
                x.FullName
            })
            .ToListAsync();

        if (assignedLawyers.Count == 0)
        {
            return;
        }

        foreach (var lawyer in assignedLawyers)
        {
            _db.Notifications.Add(new Notification
            {
                LawyerId = lawyer.Id,
                CaseId = caseId,
                Title = "تم إسناد قضية إليك",
                Body = $"تم إسناد القضية رقم {caseNumber} - {caseTitle} إليك.",
                TargetUrl = Url.Action("Details", "Cases", new { id = caseId }) ?? $"/Cases/Details/{caseId}",
                Type = "CaseAssigned",
                IsRead = false,
                CreatedAt = DateTime.Now
            });
        }

        await _db.SaveChangesAsync();
    }

    private async Task ValidateCaseLawyerSpecialtiesAsync(CaseCreateEditVM vm)
    {
        if (vm.CaseTypeId <= 0 || vm.LawyerAssignments.Count == 0)
        {
            return;
        }

        var allowedLawyerIds = await _db.Lawyers
            .AsNoTracking()
            .Where(x => x.IsActive && x.Specialties.Any(s => s.CaseTypeId == vm.CaseTypeId))
            .Select(x => x.Id)
            .ToListAsync();

        var invalidLawyers = vm.LawyerAssignments
            .Select(x => x.LawyerId)
            .Where(x => !allowedLawyerIds.Contains(x))
            .Distinct()
            .ToList();

        if (invalidLawyers.Count == 0)
        {
            return;
        }

        var caseTypeName = await _db.Lookups
            .Where(x => x.Id == vm.CaseTypeId)
            .Select(x => x.NameAr)
            .FirstOrDefaultAsync() ?? "نوع القضية";

        ModelState.AddModelError(nameof(vm.LawyerAssignments), $"فيه محامي/محامين غير متخصصين في {caseTypeName}.");
        TempData["ToastError"] = $"فيه محامي/محامين غير متخصصين في {caseTypeName}.";
    }
}
