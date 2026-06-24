using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Application.ViewModels;

public class CaseLawyerAssignmentVM
{
    [Required(ErrorMessage = "المحامي مطلوب.")]
    [Range(1, int.MaxValue, ErrorMessage = "يجب اختيار محامٍ.")]
    [Display(Name = "المحامي")]
    public int LawyerId { get; set; }

    [Required(ErrorMessage = "صلاحية المحامي مطلوبة.")]
    [Range(1, int.MaxValue, ErrorMessage = "يجب اختيار صلاحية المحامي.")]
    [Display(Name = "صلاحية المحامي")]
    public int AccessLevelId { get; set; }
}
