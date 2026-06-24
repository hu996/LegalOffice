namespace LegalOffice.Domain.Entities;

public class CaseLawyer
{
    public int Id { get; set; }

    public int CaseId { get; set; }
    public LegalCase Case { get; set; } = null!;

    public int LawyerId { get; set; }
    public Lawyer Lawyer { get; set; } = null!;

    public bool IsMainLawyer { get; set; }
    public string? RoleInCase { get; set; }
    public int AccessLevelId { get; set; }
    public Lookup AccessLevel { get; set; } = null!;
}
