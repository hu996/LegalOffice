using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Application.ViewModels;

public class LawyerCreateEditVM
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "اسم المحامي مطلوب.")]
    [StringLength(200, ErrorMessage = "اسم المحامي لا يجب أن يزيد عن 200 حرف.")]
    [Display(Name = "اسم المحامي")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "رقم الهاتف")]
    [StringLength(20, ErrorMessage = "رقم الهاتف لا يجب أن يزيد عن 20 حرف.")]
    public string? Phone { get; set; }

    [Display(Name = "البريد الإلكتروني")]
    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح.")]
    public string? Email { get; set; }

    [Display(Name = "المسمى الوظيفي")]
    [StringLength(100, ErrorMessage = "المسمى الوظيفي لا يجب أن يزيد عن 100 حرف.")]
    public string? JobTitle { get; set; }

    [Display(Name = "نشط")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "البريد المرتبط بالبوابة")]
    [EmailAddress(ErrorMessage = "البريد الإلكتروني المرتبط بالبوابة غير صحيح.")]
    public string? PortalEmail { get; set; }

    [Display(Name = "كلمة المرور المؤقتة")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "كلمة المرور المؤقتة يجب ألا تقل عن 6 أحرف.")]
    public string? TemporaryPassword { get; set; }

    [Display(Name = "أنواع القضايا")]
    public List<int> SelectedCaseTypeIds { get; set; } = new();

    public IEnumerable<SelectListItem> CaseTypes { get; set; } = new List<SelectListItem>();
}
