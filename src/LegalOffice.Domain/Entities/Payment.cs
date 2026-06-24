namespace LegalOffice.Domain.Entities;

public class Payment
{
    public int Id { get; set; }

    public int CaseId { get; set; }
    public LegalCase Case { get; set; } = null!;

    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.Today;

    public int PaymentStatusId { get; set; }
    public Lookup PaymentStatus { get; set; } = null!;

    public int? PaymentMethodId { get; set; }
    public Lookup? PaymentMethod { get; set; }

    public string? ReferenceNumber { get; set; }
    public string? ReceivedByUserId { get; set; }
    public ApplicationUser? ReceivedByUser { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
