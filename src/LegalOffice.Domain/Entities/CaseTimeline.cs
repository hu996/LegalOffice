namespace LegalOffice.Domain.Entities;
public class CaseTimeline
{
    public int Id { get; set; }
    public int CaseId { get; set; }
    public LegalCase Case { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string EventType { get; set; } = "General";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
