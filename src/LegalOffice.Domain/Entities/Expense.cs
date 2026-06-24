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
}
