using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class Lookup
{
    public int Id { get; set; }

    [Required(ErrorMessage = "النوع مطلوب.")]
    [Display(Name = "النوع")]
    public string Type { get; set; } = string.Empty;

    public int LookupTypeId { get; set; }
    public LookupType? LookupType { get; set; }

    [Required(ErrorMessage = "الاسم العربي مطلوب.")]
    [StringLength(200, ErrorMessage = "الاسم العربي لا يجب أن يزيد عن 200 حرف.")]
    [Display(Name = "الاسم العربي")]
    public string NameAr { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "الاسم الإنجليزي لا يجب أن يزيد عن 200 حرف.")]
    [Display(Name = "الاسم الإنجليزي")]
    public string? NameEn { get; set; }

    [Display(Name = "نشط")]
    public bool IsActive { get; set; } = true;
}
