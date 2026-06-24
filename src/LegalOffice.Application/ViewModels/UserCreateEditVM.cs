using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LegalOffice.Application.ViewModels;

public class UserCreateEditVM
{
    public string? Id { get; set; }

    [Required(ErrorMessage = "الاسم الكامل مطلوب.")]
    [StringLength(150, ErrorMessage = "الاسم الكامل لا يجب أن يزيد عن 150 حرفًا.")]
    [Display(Name = "الاسم الكامل")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح.")]
    [Display(Name = "البريد الإلكتروني")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم المستخدم مطلوب.")]
    [StringLength(100, ErrorMessage = "اسم المستخدم لا يجب أن يزيد عن 100 حرف.")]
    [Display(Name = "اسم المستخدم")]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "كلمة المرور")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "كلمة المرور يجب ألا تقل عن 6 أحرف.")]
    public string? Password { get; set; }

    [Display(Name = "تأكيد كلمة المرور")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "تأكيد كلمة المرور يجب ألا يقل عن 6 أحرف.")]
    [Compare(nameof(Password), ErrorMessage = "كلمتا المرور غير متطابقتين.")]
    public string? ConfirmPassword { get; set; }

    [Display(Name = "الربط بالمحامي")]
    public int? LawyerId { get; set; }

    [Display(Name = "نوع المستخدم")]
    public int? UserTypeId { get; set; }

    [Display(Name = "الإدارة")]
    public int? DepartmentId { get; set; }

    [Required(ErrorMessage = "الدور مطلوب.")]
    [Display(Name = "الدور")]
    public string RoleName { get; set; } = string.Empty;

    [Display(Name = "نشط")]
    public bool IsActive { get; set; } = true;

    public List<SelectListItem> Roles { get; set; } = new();
    public List<SelectListItem> UserTypes { get; set; } = new();
    public List<SelectListItem> Departments { get; set; } = new();
    public List<SelectListItem> Lawyers { get; set; } = new();
}

public class UserIndexItemVM
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? RoleName { get; set; }
    public string? UserTypeName { get; set; }
    public string? DepartmentName { get; set; }
    public string? LawyerName { get; set; }
    public bool IsActive { get; set; }
}
