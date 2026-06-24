namespace LegalOffice.Domain.Entities;

public class MeetingTask
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public Meeting Meeting { get; set; } = null!;
    public int TaskId { get; set; }
    public LegalTask Task { get; set; } = null!;
}
