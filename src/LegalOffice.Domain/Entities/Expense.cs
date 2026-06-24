namespace LegalOffice.Domain.Entities;

public class Expense
{
    public int Id { get; set; }
    public int CaseId { get; set; }
    public LegalCase Case { get; set; } = null!;
    public int? ExpenseTypeId { get; set; }
    public Lookup? ExpenseType { get; set; }
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; } = DateTime.Today;
    public string? Notes { get; set; }

    public int? StatusLookupId { get; set; }
    public Lookup? StatusLookup { get; set; }

    public string? SubmittedByUserId { get; set; }
    public ApplicationUser? SubmittedByUser { get; set; }

    public string? ApprovedByUserId { get; set; }
    public ApplicationUser? ApprovedByUser { get; set; }

    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
}
