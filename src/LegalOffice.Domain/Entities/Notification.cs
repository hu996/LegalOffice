using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class Notification
{
    public int Id { get; set; }

    public int LawyerId { get; set; }
    public Lawyer Lawyer { get; set; } = null!;

    public int? CaseId { get; set; }
    public LegalCase? Case { get; set; }

    [Required(ErrorMessage = "عنوان الإشعار مطلوب.")]
    [MaxLength(200, ErrorMessage = "عنوان الإشعار لا يجب أن يزيد عن 200 حرف.")]
    [Display(Name = "العنوان")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "محتوى الإشعار مطلوب.")]
    [MaxLength(500, ErrorMessage = "محتوى الإشعار لا يجب أن يزيد عن 500 حرف.")]
    [Display(Name = "المحتوى")]
    public string Body { get; set; } = string.Empty;

    [Required(ErrorMessage = "رابط الإشعار مطلوب.")]
    [MaxLength(500, ErrorMessage = "رابط الإشعار لا يجب أن يزيد عن 500 حرف.")]
    [Display(Name = "الرابط")]
    public string TargetUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "نوع الإشعار مطلوب.")]
    [MaxLength(80, ErrorMessage = "نوع الإشعار لا يجب أن يزيد عن 80 حرفًا.")]
    [Display(Name = "النوع")]
    public string Type { get; set; } = "CaseAssigned";

    [Display(Name = "مقروء")]
    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ReadAt { get; set; }
}
