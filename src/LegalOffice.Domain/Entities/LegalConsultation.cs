using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class LegalConsultation
{
    public int Id { get; set; }

    [Required(ErrorMessage = "رقم الاستشارة مطلوب.")]
    [Display(Name = "رقم الاستشارة")]
    public string ConsultationNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "عنوان الاستشارة مطلوب.")]
    [Display(Name = "عنوان الاستشارة")]
    public string Title { get; set; } = string.Empty;

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public int AssignedLawyerId { get; set; }
    public Lawyer AssignedLawyer { get; set; } = null!;

    public int ConsultationTypeLookupId { get; set; }
    public Lookup ConsultationTypeLookup { get; set; } = null!;

    public int ConsultationStatusLookupId { get; set; }
    public Lookup ConsultationStatusLookup { get; set; } = null!;

    [Display(Name = "تاريخ الطلب")]
    public DateTime RequestDate { get; set; } = DateTime.Today;

    [Display(Name = "تاريخ الرد")]
    public DateTime? ResponseDate { get; set; }

    [Display(Name = "أتعاب الاستشارة")]
    public decimal ConsultationFees { get; set; }

    [StringLength(1000)]
    public string? Subject { get; set; }

    [StringLength(4000)]
    public string? LegalOpinion { get; set; }

    [StringLength(2000)]
    public string? Notes { get; set; }

    public int? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public int? DepartmentId { get; set; }
    public Lookup? Department { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
