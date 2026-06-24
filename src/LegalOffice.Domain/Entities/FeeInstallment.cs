namespace LegalOffice.Domain.Entities;

public class FeeInstallment
{
    public int Id { get; set; }
    public int FeeAgreementId { get; set; }
    public FeeAgreement FeeAgreement { get; set; } = null!;
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public int StatusLookupId { get; set; }
    public Lookup StatusLookup { get; set; } = null!;
    public DateTime? PaidDate { get; set; }
}
