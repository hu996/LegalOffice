using Microsoft.AspNetCore.Mvc.Rendering;

namespace LegalOffice.Application.ViewModels;

public class SidebarMenuVM
{
    public IReadOnlyList<SidebarMenuGroupVM> Groups { get; set; } = Array.Empty<SidebarMenuGroupVM>();
}

public class SidebarMenuGroupVM
{
    public string Title { get; set; } = string.Empty;
    public IReadOnlyList<SidebarMenuItemVM> Items { get; set; } = Array.Empty<SidebarMenuItemVM>();
}

public class SidebarMenuItemVM
{
    public string Label { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public string Action { get; set; } = "Index";
    public string PermissionCode { get; set; } = string.Empty;
}

public class ReportFiltersVM
{
    public int? CourtId { get; set; }
    public int? LawyerId { get; set; }
    public int? CaseTypeId { get; set; }
    public int? CaseStatusId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Format { get; set; }

    public IEnumerable<SelectListItem> Courts { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Lawyers { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> CaseTypes { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> CaseStatuses { get; set; } = new List<SelectListItem>();
}

public class ReportCountRowVM
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ReportSummaryVM
{
    public ReportFiltersVM Filters { get; set; } = new();
    public int TotalCases { get; set; }
    public int OpenCases { get; set; }
    public int ClosedCases { get; set; }
    public decimal TotalFees { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalRemaining { get; set; }
    public List<ReportCaseRowVM> Cases { get; set; } = new();
    public List<ReportCountRowVM> ByCourt { get; set; } = new();
    public List<ReportCountRowVM> ByType { get; set; } = new();
    public List<ReportCountRowVM> ByStatus { get; set; } = new();
}

public class ReportLawyerRowVM
{
    public int LawyerId { get; set; }
    public string LawyerName { get; set; } = string.Empty;
    public int CasesCount { get; set; }
    public int OpenCases { get; set; }
    public int ClosedCases { get; set; }
}

public class ReportLawyerVM
{
    public ReportFiltersVM Filters { get; set; } = new();
    public List<ReportLawyerRowVM> Rows { get; set; } = new();
    public List<ReportCaseRowVM> Cases { get; set; } = new();
    public int TotalCases { get; set; }
}

public class ReportTypeRowVM
{
    public int TypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ReportStatusRowVM
{
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ReportDistributionVM
{
    public ReportFiltersVM Filters { get; set; } = new();
    public List<ReportCaseRowVM> Cases { get; set; } = new();
    public List<ReportTypeRowVM> ByType { get; set; } = new();
    public List<ReportStatusRowVM> ByStatus { get; set; } = new();
    public List<ReportCountRowVM> ByCourt { get; set; } = new();
}

public class ReportCaseRowVM
{
    public int Id { get; set; }
    public string CaseNumber { get; set; } = string.Empty;
    public int CaseYear { get; set; }
    public string CaseType { get; set; } = string.Empty;
    public string Court { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Lawyers { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public decimal FeesAmount { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Remaining => Math.Max(FeesAmount - TotalPaid, 0);
}

public class CaseReportLawyerRowVM
{
    public string LawyerName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string AccessLevel { get; set; } = string.Empty;
}

public class CaseReportHearingRowVM
{
    public DateTime HearingDate { get; set; }
    public string HearingStatus { get; set; } = string.Empty;
    public string? CourtDecision { get; set; }
    public DateTime? NextHearingDate { get; set; }
    public string? NextRequirements { get; set; }
    public string? Notes { get; set; }
}

public class CaseReportPaymentRowVM
{
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Method { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}

public class CaseReportDocumentRowVM
{
    public DateTime UploadedAt { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class CaseReportMessageRowVM
{
    public DateTime CreatedAt { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsSent { get; set; }
    public string MessageText { get; set; } = string.Empty;
    public string? ProviderResponse { get; set; }
}

public class CaseReportActivityVM
{
    public DateTime Date { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CaseReportVM
{
    public int Id { get; set; }
    public string CaseNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string CaseType { get; set; } = string.Empty;
    public string CaseStatus { get; set; } = string.Empty;
    public string? Court { get; set; }
    public string? Circuit { get; set; }
    public string? OpponentName { get; set; }
    public string? OpponentLawyer { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    public int CaseYear { get; set; }
    public decimal FeesAmount { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Remaining { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastStageChangedAt { get; set; }
    public List<CaseReportLawyerRowVM> Lawyers { get; set; } = new();
    public List<CaseReportHearingRowVM> Hearings { get; set; } = new();
    public List<CaseReportPaymentRowVM> Payments { get; set; } = new();
    public List<CaseReportDocumentRowVM> Documents { get; set; } = new();
    public List<CaseReportMessageRowVM> Messages { get; set; } = new();
    public List<CaseReportActivityVM> Activity { get; set; } = new();
}

public class ConsultationReportActivityVM
{
    public DateTime Date { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class ConsultationReportVM
{
    public int Id { get; set; }
    public string ConsultationNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string AssignedLawyer { get; set; } = string.Empty;
    public string ConsultationType { get; set; } = string.Empty;
    public string ConsultationStatus { get; set; } = string.Empty;
    public string? Branch { get; set; }
    public string? Department { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? ResponseDate { get; set; }
    public decimal ConsultationFees { get; set; }
    public string? Subject { get; set; }
    public string? LegalOpinion { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ConsultationReportActivityVM> Activity { get; set; } = new();
}
