using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Application.ViewModels;

public class HearingCreateEditVM
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "القضية مطلوبة.")]
    [Range(1, int.MaxValue, ErrorMessage = "يجب اختيار القضية.")]
    [Display(Name = "القضية")]
    public int CaseId { get; set; }

    [Required(ErrorMessage = "تاريخ الجلسة مطلوب.")]
    [Display(Name = "تاريخ الجلسة")]
    public DateTime HearingDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "حالة الجلسة مطلوبة.")]
    [Range(1, int.MaxValue, ErrorMessage = "يجب اختيار حالة الجلسة.")]
    [Display(Name = "حالة الجلسة")]
    public int HearingStatusId { get; set; }

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

    public IEnumerable<SelectListItem> HearingStatuses { get; set; } = new List<SelectListItem>();
}
