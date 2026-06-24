namespace LegalOffice.Domain.Entities;
public class MessageTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Channel { get; set; } = "WhatsApp";
    public string Body { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
