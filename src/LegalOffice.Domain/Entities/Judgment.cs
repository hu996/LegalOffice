using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class Judgment
{
    public int Id { get; set; }
    public int CaseId { get; set; }
    public LegalCase Case { get; set; } = null!;
    public DateTime JudgmentDate { get; set; } = DateTime.Today;
    public int CourtLevelLookupId { get; set; }
    public Lookup CourtLevelLookup { get; set; } = null!;
    [StringLength(2000)]
    public string? JudgmentSummary { get; set; }
    public decimal? JudgmentAmount { get; set; }
    public bool IsFinal { get; set; }
    [StringLength(2000)]
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public ICollection<ExecutionCase> ExecutionCases { get; set; } = new List<ExecutionCase>();
}
