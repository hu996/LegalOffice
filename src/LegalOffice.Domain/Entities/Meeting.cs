using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class Meeting
{
    public int Id { get; set; }
    [Required, StringLength(200)]
    public string Subject { get; set; } = string.Empty;
    public int? ClientId { get; set; }
    public Client? Client { get; set; }
    public int? CaseId { get; set; }
    public LegalCase? Case { get; set; }
    public string AssignedUserId { get; set; } = string.Empty;
    public ApplicationUser AssignedUser { get; set; } = null!;
    public DateTime MeetingDate { get; set; } = DateTime.Now;
    [StringLength(4000)]
    public string? MeetingResult { get; set; }
    [StringLength(4000)]
    public string? Notes { get; set; }
    public int StatusLookupId { get; set; }
    public Lookup StatusLookup { get; set; } = null!;
    public ICollection<MeetingTask> MeetingTasks { get; set; } = new List<MeetingTask>();
}
