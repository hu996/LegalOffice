using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Application.ViewModels;

public class BranchListItemVM
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public string? ManagerName { get; set; }
}

public class BranchEditVM
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "اسم الفرع مطلوب.")]
    [Display(Name = "اسم الفرع")]
    public string NameAr { get; set; } = string.Empty;

    [Display(Name = "العنوان")]
    public string? Address { get; set; }

    [Display(Name = "رقم الهاتف")]
    public string? Phone { get; set; }

    [Display(Name = "مدير الفرع")]
    public string? ManagerUserId { get; set; }

    [Display(Name = "نشط")]
    public bool IsActive { get; set; } = true;

    public IEnumerable<SelectListItem> Managers { get; set; } = new List<SelectListItem>();
}

public class LegalTaskListItemVM
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? RelatedCaseNumber { get; set; }
    public string AssignedToUserName { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public string PriorityName { get; set; } = string.Empty;
    public string TaskTypeName { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class LegalTaskEditVM
{
    public int? Id { get; set; }

    [Display(Name = "نوع القضية")]
    public int? CaseTypeId { get; set; }

    [Required(ErrorMessage = "عنوان المهمة مطلوب.")]
    [Display(Name = "عنوان المهمة")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "الوصف")]
    public string? Description { get; set; }

    [Display(Name = "القضية المرتبطة")]
    public int? RelatedCaseId { get; set; }

    [Required(ErrorMessage = "المسؤول عن المهمة مطلوب.")]
    [Display(Name = "مُسندة إلى")]
    public string AssignedToUserId { get; set; } = string.Empty;

    [Display(Name = "تاريخ الاستحقاق")]
    public DateTime DueDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "الأولوية مطلوبة.")]
    [Display(Name = "الأولوية")]
    public int PriorityLookupId { get; set; }

    [Required(ErrorMessage = "حالة المهمة مطلوبة.")]
    [Display(Name = "الحالة")]
    public int StatusLookupId { get; set; }

    [Required(ErrorMessage = "نوع المهمة مطلوب.")]
    [Display(Name = "نوع المهمة")]
    public int TaskTypeLookupId { get; set; }

    public IEnumerable<SelectListItem> Cases { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> CaseTypes { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Assignees { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Priorities { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Types { get; set; } = new List<SelectListItem>();
}

public class AuditLogListItemVM
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ConflictCheckVM
{
    public int? Id { get; set; }

    [Display(Name = "نوع القضية")]
    public int? CaseTypeId { get; set; }

    [Required(ErrorMessage = "اسم العميل مطلوب.")]
    [Display(Name = "اسم العميل")]
    public string ClientName { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم الخصم مطلوب.")]
    [Display(Name = "اسم الخصم")]
    public string OpponentName { get; set; } = string.Empty;

    [Display(Name = "الرقم القومي")]
    public string? NationalId { get; set; }

    [Display(Name = "رقم الهاتف")]
    public string? Phone { get; set; }

    [Display(Name = "القضية المرتبطة")]
    public int? CaseId { get; set; }

    [Required(ErrorMessage = "نتيجة الفحص مطلوبة.")]
    [Display(Name = "النتيجة")]
    public int ResultStatusLookupId { get; set; }

    [Display(Name = "الملاحظات")]
    public string? Notes { get; set; }

    public IEnumerable<SelectListItem> Cases { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> CaseTypes { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Results { get; set; } = new List<SelectListItem>();
}
