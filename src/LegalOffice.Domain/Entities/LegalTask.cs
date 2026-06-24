using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class LegalTask
{
    public int Id { get; set; }

    [Required(ErrorMessage = "عنوان المهمة مطلوب.")]
    [StringLength(200, ErrorMessage = "عنوان المهمة لا يجب أن يزيد عن 200 حرف.")]
    [Display(Name = "عنوان المهمة")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "وصف المهمة لا يجب أن يزيد عن 2000 حرف.")]
    [Display(Name = "الوصف")]
    public string? Description { get; set; }

    public int? RelatedCaseId { get; set; }
    public LegalCase? RelatedCase { get; set; }

    [Display(Name = "مُسندة إلى")]
    public string AssignedToUserId { get; set; } = string.Empty;
    public ApplicationUser AssignedToUser { get; set; } = null!;

    [Display(Name = "أنشئت بواسطة")]
    public string CreatedByUserId { get; set; } = string.Empty;
    public ApplicationUser CreatedByUser { get; set; } = null!;

    [Display(Name = "تاريخ الاستحقاق")]
    public DateTime DueDate { get; set; }

    public int PriorityLookupId { get; set; }
    public Lookup PriorityLookup { get; set; } = null!;

    public int StatusLookupId { get; set; }
    public Lookup StatusLookup { get; set; } = null!;

    public int TaskTypeLookupId { get; set; }
    public Lookup TaskTypeLookup { get; set; } = null!;

    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    [Display(Name = "محذوف")]
    public bool IsDeleted { get; set; }
}
