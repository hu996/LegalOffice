using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class LookupType
{
    public int Id { get; set; }

    [Required(ErrorMessage = "الكود مطلوب.")]
    [StringLength(100, ErrorMessage = "الكود لا يجب أن يزيد عن 100 حرف.")]
    [Display(Name = "الكود")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "الاسم العربي مطلوب.")]
    [StringLength(200, ErrorMessage = "الاسم العربي لا يجب أن يزيد عن 200 حرف.")]
    [Display(Name = "الاسم العربي")]
    public string NameAr { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "الاسم الإنجليزي لا يجب أن يزيد عن 200 حرف.")]
    [Display(Name = "الاسم الإنجليزي")]
    public string? NameEn { get; set; }

    [Display(Name = "نشط")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "ترتيب")]
    public int SortOrder { get; set; }

    public ICollection<Lookup> Lookups { get; set; } = new List<Lookup>();
}
