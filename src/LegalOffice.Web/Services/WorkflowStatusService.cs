using LegalOffice.Domain.Entities;
using LegalOffice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LegalOffice.Web.Services;

public class WorkflowStatusService : IWorkflowStatusService
{
    private static readonly IReadOnlyDictionary<string, string[]> Sequences = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
    {
        ["CaseStatus"] = ["New", "InProgress", "Deferred", "Reserved", "Judged", "Closed"],
        ["HearingStatus"] = ["Upcoming", "Done", "Deferred", "Cancelled", "Judged"],
        ["TaskStatus"] = ["New", "InProgress", "Completed", "Overdue", "Cancelled"],
        ["PaymentStatus"] = ["Recorded", "Received", "Pending", "Cancelled"],
        ["ConsultationStatus"] = ["New", "UnderReview", "WaitingClient", "Responded", "Closed"],
        ["ContractStatus"] = ["Draft", "Review", "Approved", "Active", "Expired", "Cancelled"],
        ["PowerStatus"] = ["Valid", "Expired", "Cancelled"],
        ["ExecutionStatus"] = ["NotStarted", "InProgress", "Notified", "Seized", "Collected", "Failed", "Closed"],
        ["MeetingStatus"] = ["Scheduled", "Done", "Deferred", "Cancelled"],
        ["InstallmentStatus"] = ["Due", "Paid", "Late", "Cancelled"],
        ["ExpenseStatus"] = ["Draft", "Review", "Approved", "Rejected"],
        ["ConflictCheckStatus"] = ["None", "Possible", "Confirmed", "Review"]
    };

    private readonly AppDbContext _db;

    public WorkflowStatusService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<int?> GetInitialStatusIdAsync(string lookupType)
    {
        var ordered = await GetOrderedItemsAsync(lookupType);
        return ordered.FirstOrDefault()?.Id;
    }

    public async Task<List<SelectListItem>> GetSequentialOptionsAsync(string lookupType, int? currentStatusId)
    {
        var ordered = await GetOrderedItemsAsync(lookupType);
        if (ordered.Count == 0)
        {
            return new List<SelectListItem>();
        }

        if (currentStatusId is null || currentStatusId <= 0)
        {
            var first = ordered.First();
            return [ToItem(first, first.Id)];
        }

        var currentIndex = ordered.FindIndex(x => x.Id == currentStatusId.Value);
        if (currentIndex < 0)
        {
            return ordered.Select(x => ToItem(x, currentStatusId.Value)).ToList();
        }

        var allowedCount = Math.Min(currentIndex + 2, ordered.Count);
        return ordered.Take(allowedCount)
            .Select(x => ToItem(x, currentStatusId.Value))
            .ToList();
    }

    public async Task<bool> ValidateSequentialTransitionAsync(
        string lookupType,
        int? currentStatusId,
        int selectedStatusId,
        ModelStateDictionary modelState,
        string fieldName,
        string entityLabel)
    {
        var ordered = await GetOrderedItemsAsync(lookupType);
        if (ordered.Count == 0)
        {
            return true;
        }

        if (selectedStatusId <= 0)
        {
            modelState.AddModelError(fieldName, $"حالة {entityLabel} مطلوبة.");
            return false;
        }

        var selectedIndex = ordered.FindIndex(x => x.Id == selectedStatusId);
        if (selectedIndex < 0)
        {
            modelState.AddModelError(fieldName, $"الحالة المختارة غير متاحة لـ {entityLabel}.");
            return false;
        }

        if (currentStatusId is null || currentStatusId <= 0)
        {
            if (selectedIndex != 0)
            {
                modelState.AddModelError(fieldName, $"لا يمكن بدء {entityLabel} إلا بالحالة الأولى.");
                return false;
            }

            return true;
        }

        var currentIndex = ordered.FindIndex(x => x.Id == currentStatusId.Value);
        if (currentIndex < 0)
        {
            return true;
        }

        if (selectedIndex == currentIndex)
        {
            return true;
        }

        if (selectedIndex == currentIndex + 1)
        {
            return true;
        }

        modelState.AddModelError(fieldName, $"لا يمكن تخطي تسلسل حالات {entityLabel}.");
        return false;
    }

    private async Task<List<Lookup>> GetOrderedItemsAsync(string lookupType)
    {
        if (!Sequences.TryGetValue(lookupType, out var order))
        {
            return await _db.Lookups.AsNoTracking()
                .Where(x => x.Type == lookupType && x.IsActive)
                .OrderBy(x => x.NameAr)
                .ToListAsync();
        }

        var lookups = await _db.Lookups.AsNoTracking()
            .Where(x => x.Type == lookupType && x.IsActive && order.Contains(x.NameEn!))
            .ToListAsync();

        return order.Select(name => lookups.FirstOrDefault(x => string.Equals(x.NameEn, name, StringComparison.OrdinalIgnoreCase)))
            .Where(x => x != null)
            .Cast<Lookup>()
            .ToList();
    }

    private static SelectListItem ToItem(Lookup lookup, int? selectedId)
    {
        return new SelectListItem
        {
            Text = lookup.NameAr,
            Value = lookup.Id.ToString(),
            Selected = lookup.Id == selectedId
        };
    }
}
