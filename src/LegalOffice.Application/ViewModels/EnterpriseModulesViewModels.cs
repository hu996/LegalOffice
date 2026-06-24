using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Application.ViewModels;

public class LegalConsultationVM
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "رقم الاستشارة مطلوب.")]
    [Display(Name = "رقم الاستشارة")]
    public string ConsultationNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "عنوان الاستشارة مطلوب.")]
    [Display(Name = "عنوان الاستشارة")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "العميل مطلوب.")]
    [Display(Name = "العميل")]
    public int ClientId { get; set; }

    [Required(ErrorMessage = "المستشار مطلوب.")]
    [Display(Name = "المستشار")]
    public int AssignedLawyerId { get; set; }

    [Required(ErrorMessage = "نوع الاستشارة مطلوب.")]
    [Display(Name = "نوع الاستشارة")]
    public int ConsultationTypeLookupId { get; set; }

    [Required(ErrorMessage = "حالة الاستشارة مطلوبة.")]
    [Display(Name = "حالة الاستشارة")]
    public int ConsultationStatusLookupId { get; set; }

    [Display(Name = "تاريخ الطلب")]
    public DateTime RequestDate { get; set; } = DateTime.Today;

    [Display(Name = "تاريخ الرد")]
    public DateTime? ResponseDate { get; set; }

    [Display(Name = "أتعاب الاستشارة")]
    public decimal ConsultationFees { get; set; }

    [Display(Name = "الموضوع")]
    public string? Subject { get; set; }

    [Display(Name = "الرأي القانوني")]
    public string? LegalOpinion { get; set; }

    [Display(Name = "ملاحظات")]
    public string? Notes { get; set; }

    [Display(Name = "الفرع")]
    public int? BranchId { get; set; }

    [Display(Name = "القسم")]
    public int? DepartmentId { get; set; }

    public IEnumerable<SelectListItem> Clients { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Lawyers { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Types { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Branches { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
}

public class ContractVM
{
    public int? Id { get; set; }

    [Display(Name = "رقم العقد")]
    [Required(ErrorMessage = "رقم العقد مطلوب.")]
    public string ContractNumber { get; set; } = string.Empty;

    [Display(Name = "عنوان العقد")]
    [Required(ErrorMessage = "عنوان العقد مطلوب.")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "نوع العقد")]
    [Required(ErrorMessage = "نوع العقد مطلوب.")]
    public int? ContractTypeLookupId { get; set; }

    [Display(Name = "العميل")]
    [Required(ErrorMessage = "العميل مطلوب.")]
    public int? ClientId { get; set; }

    [Display(Name = "المستشار")]
    [Required(ErrorMessage = "المستشار مطلوب.")]
    public int? AssignedLawyerId { get; set; }

    [Display(Name = "تاريخ البداية")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Display(Name = "تاريخ النهاية")]
    public DateTime? EndDate { get; set; }

    [Display(Name = "قيمة العقد")]
    [Required(ErrorMessage = "قيمة العقد مطلوبة.")]
    [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "قيمة العقد يجب أن تكون أكبر من صفر.")]
    public decimal? ContractValue { get; set; }

    [Display(Name = "حالة العقد")]
    [Required(ErrorMessage = "حالة العقد مطلوبة.")]
    public int? StatusLookupId { get; set; }

    [Display(Name = "ملاحظات")]
    public string? Notes { get; set; }

    [Display(Name = "الفرع")]
    public int? BranchId { get; set; }

    [Display(Name = "القسم")]
    public int? DepartmentId { get; set; }

    public IEnumerable<SelectListItem> Clients { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Lawyers { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Types { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Branches { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
}

public class PowerOfAttorneyVM
{
    public int? Id { get; set; }
    [Display(Name = "رقم التوكيل")]
    [Required(ErrorMessage = "رقم التوكيل مطلوب.")]
    public string PowerNumber { get; set; } = string.Empty;

    [Display(Name = "العميل")]
    [Required(ErrorMessage = "العميل مطلوب.")]
    public int? ClientId { get; set; }

    [Display(Name = "نوع التوكيل")]
    [Required(ErrorMessage = "نوع التوكيل مطلوب.")]
    public int? TypeLookupId { get; set; }

    [Display(Name = "تاريخ الإصدار")]
    public DateTime IssueDate { get; set; } = DateTime.Today;

    [Display(Name = "تاريخ الانتهاء")]
    public DateTime? ExpiryDate { get; set; }

    [Display(Name = "جهة التسجيل")]
    public string? RegistrationOffice { get; set; }

    [Display(Name = "ملاحظات")]
    public string? Notes { get; set; }

    [Display(Name = "المرفقات")]
    public List<IFormFile>? Attachments { get; set; } = new();

    [Display(Name = "حالة التوكيل")]
    [Required(ErrorMessage = "حالة التوكيل مطلوبة.")]
    public int? StatusLookupId { get; set; }

    [Display(Name = "القضية")]
    public int? CaseId { get; set; }
    public IEnumerable<SelectListItem> Clients { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Types { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Cases { get; set; } = new List<SelectListItem>();
}
