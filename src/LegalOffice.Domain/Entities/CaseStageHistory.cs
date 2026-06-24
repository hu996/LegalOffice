namespace LegalOffice.Domain.Entities;

public class CaseStageHistory
{
    public int Id { get; set; }
    public int CaseId { get; set; }
    public LegalCase Case { get; set; } = null!;
    public int? FromStageLookupId { get; set; }
    public Lookup? FromStageLookup { get; set; }
    public int ToStageLookupId { get; set; }
    public Lookup ToStageLookup { get; set; } = null!;
    public string ChangedByUserId { get; set; } = string.Empty;
    public ApplicationUser ChangedByUser { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.Now;
}
