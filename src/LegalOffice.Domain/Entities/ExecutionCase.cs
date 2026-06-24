using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class ExecutionCase
{
    public int Id { get; set; }
    public int JudgmentId { get; set; }
    public Judgment Judgment { get; set; } = null!;
    public int ExecutionStatusLookupId { get; set; }
    public Lookup ExecutionStatusLookup { get; set; } = null!;
    [StringLength(200)]
    public string? ExecutionOfficer { get; set; }
    [StringLength(2000)]
    public string? ExecutionNotes { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
