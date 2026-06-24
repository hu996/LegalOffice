using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Application.ViewModels;

public class JudgmentVM
{
    public int? Id { get; set; }

    [Display(Name = "نوع القضية")]
    public int? CaseTypeId { get; set; }

    [Required(ErrorMessage = "القضية مطلوبة.")]
    [Display(Name = "القضية")]
    public int? CaseId { get; set; }

    [Display(Name = "تاريخ الحكم")]
    public DateTime JudgmentDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "درجة المحكمة مطلوبة.")]
    [Display(Name = "درجة المحكمة")]
    public int CourtLevelLookupId { get; set; }

    [Display(Name = "ملخص الحكم")]
    [StringLength(2000, ErrorMessage = "ملخص الحكم لا يجوز أن يتجاوز 2000 حرف.")]
    public string? JudgmentSummary { get; set; }

    [Display(Name = "قيمة الحكم")]
    public decimal? JudgmentAmount { get; set; }

    [Display(Name = "حكم نهائي")]
    public bool IsFinal { get; set; }

    [Display(Name = "ملاحظات")]
    [StringLength(2000, ErrorMessage = "الملاحظات لا يجوز أن تتجاوز 2000 حرف.")]
    public string? Notes { get; set; }

    public IEnumerable<SelectListItem> Cases { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> CaseTypes { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> CourtLevels { get; set; } = new List<SelectListItem>();
}

public class ExecutionCaseVM
{
    public int? Id { get; set; }

    [Display(Name = "نوع القضية")]
    public int? CaseTypeId { get; set; }

    [Required(ErrorMessage = "الحكم مطلوب.")]
    [Display(Name = "الحكم")]
    public int? JudgmentId { get; set; }

    [Required(ErrorMessage = "حالة التنفيذ مطلوبة.")]
    [Display(Name = "حالة التنفيذ")]
    public int ExecutionStatusLookupId { get; set; }

    [Display(Name = "مأمور التنفيذ")]
    [StringLength(200, ErrorMessage = "اسم مأمور التنفيذ لا يجوز أن يتجاوز 200 حرف.")]
    public string? ExecutionOfficer { get; set; }

    [Display(Name = "ملاحظات التنفيذ")]
    [StringLength(2000, ErrorMessage = "ملاحظات التنفيذ لا يجوز أن تتجاوز 2000 حرف.")]
    public string? ExecutionNotes { get; set; }

    [Display(Name = "تاريخ البداية")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Display(Name = "تاريخ النهاية")]
    public DateTime? EndDate { get; set; }

    public IEnumerable<SelectListItem> Judgments { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> CaseTypes { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
}

public class MeetingVM
{
    public int? Id { get; set; }

    [Display(Name = "نوع القضية")]
    public int? CaseTypeId { get; set; }

    [Required(ErrorMessage = "موضوع الاجتماع مطلوب.")]
    [Display(Name = "موضوع الاجتماع")]
    [StringLength(200, ErrorMessage = "موضوع الاجتماع لا يجوز أن يتجاوز 200 حرف.")]
    public string Subject { get; set; } = string.Empty;

    [Display(Name = "العميل")]
    public int? ClientId { get; set; }

    [Display(Name = "القضية")]
    public int? CaseId { get; set; }

    [Required(ErrorMessage = "المسؤول عن الاجتماع مطلوب.")]
    [Display(Name = "المسؤول")]
    public string AssignedUserId { get; set; } = string.Empty;

    [Display(Name = "تاريخ الاجتماع")]
    public DateTime MeetingDate { get; set; } = DateTime.Now;

    [Display(Name = "نتيجة الاجتماع")]
    [StringLength(4000, ErrorMessage = "نتيجة الاجتماع لا يجوز أن تتجاوز 4000 حرف.")]
    public string? MeetingResult { get; set; }

    [Display(Name = "ملاحظات")]
    [StringLength(4000, ErrorMessage = "الملاحظات لا يجوز أن تتجاوز 4000 حرف.")]
    public string? Notes { get; set; }

    [Required(ErrorMessage = "حالة الاجتماع مطلوبة.")]
    [Display(Name = "الحالة")]
    public int StatusLookupId { get; set; }

    public IEnumerable<SelectListItem> Clients { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Cases { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> CaseTypes { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Users { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
}
