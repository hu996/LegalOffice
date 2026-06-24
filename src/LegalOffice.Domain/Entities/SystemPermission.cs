using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class SystemPermission
{
    public int Id { get; set; }

    [Required(ErrorMessage = "كود الصلاحية مطلوب.")]
    [StringLength(120, ErrorMessage = "كود الصلاحية لا يجب أن يزيد عن 120 حرفًا.")]
    [Display(Name = "كود الصلاحية")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم الصلاحية بالعربية مطلوب.")]
    [StringLength(200, ErrorMessage = "اسم الصلاحية بالعربية لا يجب أن يزيد عن 200 حرف.")]
    [Display(Name = "الاسم العربي")]
    public string NameAr { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "الاسم الإنجليزي لا يجب أن يزيد عن 200 حرف.")]
    [Display(Name = "الاسم الإنجليزي")]
    public string? NameEn { get; set; }

    [Required(ErrorMessage = "اسم الكنترولر مطلوب.")]
    [StringLength(120, ErrorMessage = "اسم الكنترولر لا يجب أن يزيد عن 120 حرفًا.")]
    [Display(Name = "الكنترولر")]
    public string Controller { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم الأكشن مطلوب.")]
    [StringLength(120, ErrorMessage = "اسم الأكشن لا يجب أن يزيد عن 120 حرفًا.")]
    [Display(Name = "الأكشن")]
    public string Action { get; set; } = string.Empty;

    [StringLength(80, ErrorMessage = "اسم المجموعة لا يجب أن يزيد عن 80 حرفًا.")]
    [Display(Name = "مجموعة القائمة")]
    public string? MenuGroup { get; set; }

    [Display(Name = "الترتيب")]
    public int SortOrder { get; set; }

    [Display(Name = "عنصر قائمة")]
    public bool IsMenuItem { get; set; }

    [Display(Name = "نشط")]
    public bool IsActive { get; set; } = true;

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
