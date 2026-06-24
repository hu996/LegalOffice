using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class FeeAgreement
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public int? CaseId { get; set; }
    public LegalCase? Case { get; set; }
    public int FeeTypeLookupId { get; set; }
    public Lookup FeeTypeLookup { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    [StringLength(2000)]
    public string? Notes { get; set; }
    public ICollection<FeeInstallment> Installments { get; set; } = new List<FeeInstallment>();
}
