using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class Lawyer
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم المحامي مطلوب.")]
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

    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public ICollection<CaseLawyer> CaseLawyers { get; set; } = new List<CaseLawyer>();
    public ICollection<LawyerSpecialty> Specialties { get; set; } = new List<LawyerSpecialty>();
}
