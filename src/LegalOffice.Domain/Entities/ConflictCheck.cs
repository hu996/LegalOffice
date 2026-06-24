using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class ConflictCheck
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم العميل مطلوب.")]
    [Display(Name = "اسم العميل")]
    public string ClientName { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم الخصم مطلوب.")]
    [Display(Name = "اسم الخصم")]
    public string OpponentName { get; set; } = string.Empty;

    [Display(Name = "الرقم القومي")]
    public string? NationalId { get; set; }

    [Display(Name = "رقم الهاتف")]
    public string? Phone { get; set; }

    public int? CaseId { get; set; }
    public LegalCase? Case { get; set; }

    public int ResultStatusLookupId { get; set; }
    public Lookup ResultStatusLookup { get; set; } = null!;

    [StringLength(1000, ErrorMessage = "الملاحظات لا يجب أن تزيد عن 1000 حرف.")]
    [Display(Name = "الملاحظات")]
    public string? Notes { get; set; }

    public string CheckedByUserId { get; set; } = string.Empty;
    public ApplicationUser CheckedByUser { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
