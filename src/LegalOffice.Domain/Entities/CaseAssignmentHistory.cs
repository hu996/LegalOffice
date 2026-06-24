namespace LegalOffice.Domain.Entities;

public class CaseAssignmentHistory
{
    public int Id { get; set; }
    public int CaseId { get; set; }
    public LegalCase Case { get; set; } = null!;
    public int LawyerId { get; set; }
    public Lawyer Lawyer { get; set; } = null!;
    public string ActionType { get; set; } = string.Empty;
    public string ChangedByUserId { get; set; } = string.Empty;
    public ApplicationUser ChangedByUser { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
