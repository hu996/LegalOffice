using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Application.ViewModels;

public class CaseCreateEditVM
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "رقم القضية مطلوب.")]
    [StringLength(100, ErrorMessage = "رقم القضية لا يجب أن يزيد عن 100 حرف.")]
    [Display(Name = "رقم القضية")]
    public string CaseNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "عنوان القضية مطلوب.")]
    [StringLength(200, ErrorMessage = "عنوان القضية لا يجب أن يزيد عن 200 حرف.")]
    [Display(Name = "العنوان")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "الوصف لا يجب أن يزيد عن 1000 حرف.")]
    [Display(Name = "الوصف")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "العميل مطلوب.")]
    [Range(1, int.MaxValue, ErrorMessage = "يجب اختيار عميل.")]
    [Display(Name = "العميل")]
    public int ClientId { get; set; }

    [Required(ErrorMessage = "نوع القضية مطلوب.")]
    [Range(1, int.MaxValue, ErrorMessage = "يجب اختيار نوع القضية.")]
    [Display(Name = "نوع القضية")]
    public int CaseTypeId { get; set; }

    [Required(ErrorMessage = "حالة القضية مطلوبة.")]
    [Range(1, int.MaxValue, ErrorMessage = "يجب اختيار حالة القضية.")]
    [Display(Name = "حالة القضية")]
    public int CaseStatusId { get; set; }

    [Display(Name = "المحكمة")]
    public int? CourtId { get; set; }

    [StringLength(100, ErrorMessage = "الدائرة لا يجب أن تزيد عن 100 حرف.")]
    [Display(Name = "الدائرة")]
    public string? Circuit { get; set; }

    [StringLength(200, ErrorMessage = "اسم الخصم لا يجب أن يزيد عن 200 حرف.")]
    [Display(Name = "اسم الخصم")]
    public string? OpponentName { get; set; }

    [StringLength(200, ErrorMessage = "اسم محامي الخصم لا يجب أن يزيد عن 200 حرف.")]
    [Display(Name = "محامي الخصم")]
    public string? OpponentLawyer { get; set; }

    [Display(Name = "تاريخ البداية")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "سنة القضية مطلوبة.")]
    [Range(2000, 2100, ErrorMessage = "سنة القضية غير صحيحة.")]
    [Display(Name = "سنة القضية")]
    public int CaseYear { get; set; } = DateTime.Today.Year;

    [Display(Name = "أتعاب القضية")]
    [Range(0, double.MaxValue, ErrorMessage = "أتعاب القضية لا يمكن أن تكون سالبة.")]
    public decimal FeesAmount { get; set; }

    [Range(1, 20, ErrorMessage = "عدد المحامين يجب أن يكون بين 1 و 20.")]
    [Display(Name = "عدد المحامين")]
    public int LawyersCount { get; set; } = 1;

    [Required(ErrorMessage = "الأولوية مطلوبة.")]
    [Range(1, int.MaxValue, ErrorMessage = "يجب اختيار الأولوية.")]
    [Display(Name = "الأولوية")]
    public int PriorityId { get; set; }

    public List<CaseLawyerAssignmentVM> LawyerAssignments { get; set; } = new();

    public IEnumerable<SelectListItem> Clients { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> CaseTypes { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> CaseStatuses { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Courts { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> AccessLevels { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Priorities { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Lawyers { get; set; } = new List<SelectListItem>();
}
