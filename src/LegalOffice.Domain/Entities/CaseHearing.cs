using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class CaseHearing
{
    public int Id { get; set; }

    public int CaseId { get; set; }
    public LegalCase Case { get; set; } = null!;

    [Required(ErrorMessage = "تاريخ الجلسة مطلوب.")]
    [Display(Name = "تاريخ الجلسة")]
    public DateTime HearingDate { get; set; }

    public int HearingStatusId { get; set; }
    public Lookup HearingStatus { get; set; } = null!;

    [StringLength(1000, ErrorMessage = "قرار المحكمة لا يجب أن يزيد عن 1000 حرف.")]
    [Display(Name = "قرار المحكمة")]
    public string? CourtDecision { get; set; }

    [StringLength(1000, ErrorMessage = "الملاحظات لا يجب أن تزيد عن 1000 حرف.")]
    [Display(Name = "الملاحظات")]
    public string? Notes { get; set; }

    [StringLength(1000, ErrorMessage = "المتطلبات القادمة لا يجب أن تزيد عن 1000 حرف.")]
    [Display(Name = "المتطلبات القادمة")]
    public string? NextRequirements { get; set; }

    [Display(Name = "تاريخ الجلسة القادمة")]
    public DateTime? NextHearingDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
