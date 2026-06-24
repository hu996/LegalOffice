namespace LegalOffice.Domain.Entities;
public class CaseDocument
{
    public int Id { get; set; }
    public int CaseId { get; set; }
    public LegalCase Case { get; set; } = null!;
    public int DocumentTypeId { get; set; }
    public Lookup DocumentType { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.Now;
}
