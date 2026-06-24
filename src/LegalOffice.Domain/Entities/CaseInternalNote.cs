using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class CaseInternalNote
{
    public int Id { get; set; }
    public int CaseId { get; set; }
    public LegalCase Case { get; set; } = null!;

    [Required(ErrorMessage = "الملاحظة مطلوبة.")]
    [StringLength(2000, ErrorMessage = "الملاحظة لا يجب أن تزيد عن 2000 حرف.")]
    [Display(Name = "الملاحظة")]
    public string Note { get; set; } = string.Empty;

    public string CreatedByUserId { get; set; } = string.Empty;
    public ApplicationUser CreatedByUser { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
