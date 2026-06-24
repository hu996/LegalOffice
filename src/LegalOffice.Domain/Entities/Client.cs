using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class Client
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم العميل مطلوب.")]
    [Display(Name = "اسم العميل")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "الموبايل")]
    [RegularExpression(@"^01[0-2,5][0-9]{8}$", ErrorMessage = "رقم الموبايل يجب أن يكون 11 رقمًا مصريًا صحيحًا.")]
    public string? Phone { get; set; }

    [Display(Name = "الواتساب")]
    [RegularExpression(@"^01[0-2,5][0-9]{8}$", ErrorMessage = "رقم الواتساب يجب أن يكون 11 رقمًا مصريًا صحيحًا.")]
    public string? WhatsAppPhone { get; set; }

    [Display(Name = "البريد الإلكتروني")]
    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "الرقم القومي مطلوب.")]
    [RegularExpression(@"^\d{14}$", ErrorMessage = "الرقم القومي يجب أن يكون 14 رقمًا.")]
    [Display(Name = "الرقم القومي")]
    public string NationalId { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "العنوان لا يجب أن يزيد عن 500 حرف.")]
    [Display(Name = "العنوان")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "نوع العميل مطلوب.")]
    [Display(Name = "نوع العميل")]
    public string ClientType { get; set; } = "فرد";

    [StringLength(1000, ErrorMessage = "الملاحظات لا يجب أن تزيد عن 1000 حرف.")]
    [Display(Name = "الملاحظات")]
    public string? Notes { get; set; }

    public int? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public ICollection<LegalCase> Cases { get; set; } = new List<LegalCase>();
}
