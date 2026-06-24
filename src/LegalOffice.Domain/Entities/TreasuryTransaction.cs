using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class TreasuryTransaction
{
    public int Id { get; set; }
    public int TreasuryId { get; set; }
    public OfficeTreasury Treasury { get; set; } = null!;
    public int TransactionTypeLookupId { get; set; }
    public Lookup TransactionTypeLookup { get; set; } = null!;
    public decimal Amount { get; set; }
    [StringLength(2000)]
    public string? Notes { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    public int? CaseId { get; set; }
    public LegalCase? Case { get; set; }
}
