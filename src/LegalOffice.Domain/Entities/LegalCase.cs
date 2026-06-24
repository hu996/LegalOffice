using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Domain.Entities;

public class LegalCase
{
    public int Id { get; set; }

    [Required(ErrorMessage = "رقم القضية مطلوب.")]
    [Display(Name = "رقم القضية")]
    public string CaseNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "عنوان القضية مطلوب.")]
    [Display(Name = "عنوان القضية")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "الوصف لا يجب أن يزيد عن 1000 حرف.")]
    [Display(Name = "الوصف")]
    public string? Description { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public int CaseTypeId { get; set; }
    public Lookup CaseType { get; set; } = null!;

    public int CaseStatusId { get; set; }
    public Lookup CaseStatus { get; set; } = null!;

    public int? CourtId { get; set; }
    public Lookup? Court { get; set; }

    [StringLength(100, ErrorMessage = "الدائرة لا يجب أن تزيد عن 100 حرف.")]
    [Display(Name = "الدائرة")]
    public string? Circuit { get; set; }

    [StringLength(200, ErrorMessage = "اسم الخصم لا يجب أن يزيد عن 200 حرف.")]
    [Display(Name = "اسم الخصم")]
    public string? OpponentName { get; set; }

    [StringLength(200, ErrorMessage = "اسم محامي الخصم لا يجب أن يزيد عن 200 حرف.")]
    [Display(Name = "محامي الخصم")]
    public string? OpponentLawyer { get; set; }

    [Display(Name = "تاريخ البداية")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Display(Name = "تاريخ الإغلاق")]
    public DateTime? ClosedDate { get; set; }

    [Display(Name = "أتعاب القضية")]
    public decimal FeesAmount { get; set; }

    public int CaseYear { get; set; }

    public string? CreatedByUserId { get; set; }
    public ApplicationUser? CreatedByUser { get; set; }

    [Display(Name = "عدد المحامين")]
    public int LawyersCount { get; set; } = 1;

    public int? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public int? DepartmentId { get; set; }
    public Lookup? Department { get; set; }

    public int? WorkflowStageLookupId { get; set; }
    public Lookup? WorkflowStageLookup { get; set; }

    public DateTime? LastStageChangedAt { get; set; }

    public int PriorityId { get; set; }
    public Lookup Priority { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<CaseLawyer> CaseLawyers { get; set; } = new List<CaseLawyer>();
    public ICollection<CaseHearing> Hearings { get; set; } = new List<CaseHearing>();
    public ICollection<CaseDocument> Documents { get; set; } = new List<CaseDocument>();
    public ICollection<CaseTimeline> Timelines { get; set; } = new List<CaseTimeline>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<CaseInternalNote> InternalNotes { get; set; } = new List<CaseInternalNote>();
    public ICollection<CaseStageHistory> StageHistory { get; set; } = new List<CaseStageHistory>();
    public ICollection<ConflictCheck> ConflictChecks { get; set; } = new List<ConflictCheck>();
}
