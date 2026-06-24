using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class PowerOfAttorney
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string PowerNumber { get; set; } = string.Empty;

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public int TypeLookupId { get; set; }
    public Lookup TypeLookup { get; set; } = null!;

    public DateTime IssueDate { get; set; } = DateTime.Today;
    public DateTime? ExpiryDate { get; set; }
    [StringLength(200)]
    public string? RegistrationOffice { get; set; }
    [StringLength(2000)]
    public string? Notes { get; set; }
    [StringLength(4000)]
    public string? FilePath { get; set; }
    public int StatusLookupId { get; set; }
    public Lookup StatusLookup { get; set; } = null!;
    public int? CaseId { get; set; }
    public LegalCase? Case { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
