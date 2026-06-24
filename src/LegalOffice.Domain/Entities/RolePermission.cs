using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class RolePermission
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم الدور مطلوب.")]
    [StringLength(256, ErrorMessage = "اسم الدور لا يجب أن يزيد عن 256 حرفًا.")]
    [Display(Name = "الدور")]
    public string RoleName { get; set; } = string.Empty;

    [Required(ErrorMessage = "الصلاحية مطلوبة.")]
    [Display(Name = "الصلاحية")]
    public int SystemPermissionId { get; set; }

    public SystemPermission SystemPermission { get; set; } = null!;
}
