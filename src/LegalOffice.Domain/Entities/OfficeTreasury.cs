namespace LegalOffice.Domain.Entities;

public class OfficeTreasury
{
    public int Id { get; set; }
    public string NameAr { get; set; } = "خزنة المكتب";
    public decimal CurrentBalance { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
