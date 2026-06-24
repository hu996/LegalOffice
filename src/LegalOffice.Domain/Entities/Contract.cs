using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class Contract
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string ContractNumber { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    public int ContractTypeLookupId { get; set; }
    public Lookup ContractTypeLookup { get; set; } = null!;

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public int AssignedLawyerId { get; set; }
    public Lawyer AssignedLawyer { get; set; } = null!;

    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime? EndDate { get; set; }
    public decimal ContractValue { get; set; }

    public int StatusLookupId { get; set; }
    public Lookup StatusLookup { get; set; } = null!;

    [StringLength(2000)]
    public string? Notes { get; set; }

    public int? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public int? DepartmentId { get; set; }
    public Lookup? Department { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<ContractVersion> Versions { get; set; } = new List<ContractVersion>();
}
