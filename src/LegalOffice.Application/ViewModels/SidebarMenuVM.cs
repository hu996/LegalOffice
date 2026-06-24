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
    public List<ReportTypeRowVM> ByType { get; set; } = new();
    public List<ReportStatusRowVM> ByStatus { get; set; } = new();
    public List<ReportCountRowVM> ByCourt { get; set; } = new();
}
