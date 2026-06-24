using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class ContractVersion
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public Contract Contract { get; set; } = null!;
    public int VersionNumber { get; set; }
    [StringLength(500)]
    public string FilePath { get; set; } = string.Empty;
    [StringLength(2000)]
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
