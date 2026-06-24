namespace LegalOffice.Domain.Entities;

public class LawyerSpecialty
{
    public int Id { get; set; }

    public int LawyerId { get; set; }
    public Lawyer Lawyer { get; set; } = null!;

    public int CaseTypeId { get; set; }
    public Lookup CaseType { get; set; } = null!;
}
