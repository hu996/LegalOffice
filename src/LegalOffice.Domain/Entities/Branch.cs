using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class Branch
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم الفرع مطلوب.")]
    [StringLength(200, ErrorMessage = "اسم الفرع لا يجب أن يزيد عن 200 حرف.")]
    [Display(Name = "اسم الفرع")]
    public string NameAr { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "العنوان لا يجب أن يزيد عن 500 حرف.")]
    [Display(Name = "العنوان")]
    public string? Address { get; set; }

    [StringLength(20, ErrorMessage = "رقم الهاتف لا يجب أن يزيد عن 20 حرف.")]
    [Display(Name = "رقم الهاتف")]
    public string? Phone { get; set; }

    public string? ManagerUserId { get; set; }
    public ApplicationUser? ManagerUser { get; set; }

    [Display(Name = "نشط")]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
