using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Application.ViewModels;

public class DocumentCreateEditVM
{
    public int CaseId { get; set; }

    [Required(ErrorMessage = "نوع المستند مطلوب.")]
    [Range(1, int.MaxValue, ErrorMessage = "يجب اختيار نوع المستند.")]
    [Display(Name = "نوع المستند")]
    public int DocumentTypeId { get; set; }

    [Required(ErrorMessage = "الملف مطلوب.")]
    [Display(Name = "الملف")]
    public IFormFile? File { get; set; }

    [StringLength(1000, ErrorMessage = "الملاحظات لا يجب أن تزيد عن 1000 حرف.")]
    [Display(Name = "الملاحظات")]
    public string? Notes { get; set; }

    public IEnumerable<SelectListItem> DocumentTypes { get; set; } = new List<SelectListItem>();
}
