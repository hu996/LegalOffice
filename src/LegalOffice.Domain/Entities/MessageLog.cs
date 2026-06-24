namespace LegalOffice.Domain.Entities;
public class MessageLog
{
    public int Id { get; set; }
    public int? CaseId { get; set; }
    public int ClientId { get; set; }
    public string Channel { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string MessageText { get; set; } = null!;
    public bool IsSent { get; set; }
    public string? ProviderResponse { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
