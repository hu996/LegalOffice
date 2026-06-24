using ClosedXML.Excel;
using LegalOffice.Application.ViewModels;
using LegalOffice.Domain.Entities;
using LegalOffice.Infrastructure.Persistence;
using LegalOffice.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;
    private readonly ICaseTypeOptionsService _caseTypeOptions;

    public ReportsController(AppDbContext db, IPermissionService permissions, ICaseTypeOptionsService caseTypeOptions)
    {
        _db = db;
        _permissions = permissions;
        _caseTypeOptions = caseTypeOptions;
    }

    public async Task<IActionResult> Index([FromQuery] ReportFiltersVM filters)
    {
        if (!await EnsureAccessAsync("Reports.Overview")) return Forbid();
        await FillFiltersAsync(filters);

        var query = BuildCaseQuery(filters);
        var summary = await BuildSummaryAsync(query, filters);

        if (!string.IsNullOrWhiteSpace(filters.Format))
        {
            return filters.Format.Equals("pdf", StringComparison.OrdinalIgnoreCase)
                ? File(BuildSummaryPdf(summary), "application/pdf", "LegalOffice-Overview-Report.pdf")
                : File(BuildSummaryExcel(summary), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LegalOffice-Overview-Report.xlsx");
        }

        return View(summary);
    }

    public async Task<IActionResult> Lawyers([FromQuery] ReportFiltersVM filters)
    {
        if (!await EnsureAccessAsync("Reports.Lawyers")) return Forbid();
        await FillFiltersAsync(filters);

        var rows = await BuildLawyerRowsAsync(filters);
        var vm = new ReportLawyerVM
        {
            Filters = filters,
            Rows = rows,
            TotalCases = rows.Sum(x => x.CasesCount)
        };

        if (!string.IsNullOrWhiteSpace(filters.Format))
        {
            return filters.Format.Equals("pdf", StringComparison.OrdinalIgnoreCase)
                ? File(BuildLawyersPdf(vm), "application/pdf", "LegalOffice-Lawyers-Report.pdf")
                : File(BuildLawyersExcel(vm), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LegalOffice-Lawyers-Report.xlsx");
        }

        return View(vm);
    }

    public async Task<IActionResult> Distribution([FromQuery] ReportFiltersVM filters)
    {
        if (!await EnsureAccessAsync("Reports.Distribution")) return Forbid();
        await FillFiltersAsync(filters);

        var vm = await BuildDistributionAsync(filters);

        if (!string.IsNullOrWhiteSpace(filters.Format))
        {
            return filters.Format.Equals("pdf", StringComparison.OrdinalIgnoreCase)
                ? File(BuildDistributionPdf(vm), "application/pdf", "LegalOffice-Distribution-Report.pdf")
                : File(BuildDistributionExcel(vm), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LegalOffice-Distribution-Report.xlsx");
        }

        return View(vm);
    }

    private async Task<bool> EnsureAccessAsync(string code) => await _permissions.HasPermissionAsync(User, code);

    private IQueryable<LegalCase> BuildCaseQuery(ReportFiltersVM filters)
    {
        var query = _db.Cases
            .AsNoTracking()
            .Include(x => x.CaseType)
            .Include(x => x.CaseStatus)
            .Include(x => x.Court)
            .Include(x => x.Payments)
            .Include(x => x.CaseLawyers)
            .ThenInclude(x => x.Lawyer)
            .AsQueryable();

        if (filters.CourtId.HasValue)
        {
            query = query.Where(x => x.CourtId == filters.CourtId);
        }

        if (filters.CaseTypeId.HasValue)
        {
            query = query.Where(x => x.CaseTypeId == filters.CaseTypeId);
        }

        if (filters.CaseStatusId.HasValue)
        {
            query = query.Where(x => x.CaseStatusId == filters.CaseStatusId);
        }

        if (filters.FromDate.HasValue)
        {
            query = query.Where(x => x.StartDate >= filters.FromDate.Value.Date);
        }

        if (filters.ToDate.HasValue)
        {
            query = query.Where(x => x.StartDate <= filters.ToDate.Value.Date);
        }

        if (filters.LawyerId.HasValue)
        {
            query = query.Where(x => x.CaseLawyers.Any(cl => cl.LawyerId == filters.LawyerId.Value));
        }

        return query;
    }

    private async Task<ReportSummaryVM> BuildSummaryAsync(IQueryable<LegalCase> query, ReportFiltersVM filters)
    {
        var cases = await query.ToListAsync();
        var totalPaid = cases.Sum(c => c.Payments.Where(p => p.PaymentStatusId > 0).Sum(p => p.Amount));

        return new ReportSummaryVM
        {
            Filters = filters,
            TotalCases = cases.Count,
            OpenCases = cases.Count(x => x.ClosedDate == null),
            ClosedCases = cases.Count(x => x.ClosedDate != null),
            TotalFees = cases.Sum(x => x.FeesAmount),
            TotalPaid = totalPaid,
            TotalRemaining = Math.Max(cases.Sum(x => x.FeesAmount) - totalPaid, 0),
            ByCourt = cases.GroupBy(x => x.Court?.NameAr ?? "بدون محكمة")
                .Select(g => new ReportCountRowVM { Label = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Label)
                .ToList(),
            ByType = cases.GroupBy(x => x.CaseType?.NameAr ?? "بدون نوع")
                .Select(g => new ReportCountRowVM { Label = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Label)
                .ToList(),
            ByStatus = cases.GroupBy(x => x.CaseStatus?.NameAr ?? "بدون حالة")
                .Select(g => new ReportCountRowVM { Label = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Label)
                .ToList()
        };
    }

    private async Task<List<ReportLawyerRowVM>> BuildLawyerRowsAsync(ReportFiltersVM filters)
    {
        var query = BuildCaseQuery(filters);

        var rows = await query
            .SelectMany(c => c.CaseLawyers.Select(cl => new
            {
                cl.LawyerId,
                LawyerName = cl.Lawyer.FullName,
                c.Id,
                IsClosed = c.ClosedDate != null
            }))
            .GroupBy(x => new { x.LawyerId, x.LawyerName })
            .Select(g => new ReportLawyerRowVM
            {
                LawyerId = g.Key.LawyerId,
                LawyerName = g.Key.LawyerName,
                CasesCount = g.Select(x => x.Id).Distinct().Count(),
                OpenCases = g.Count(x => !x.IsClosed),
                ClosedCases = g.Count(x => x.IsClosed)
            })
            .OrderByDescending(x => x.CasesCount)
            .ThenBy(x => x.LawyerName)
            .ToListAsync();

        if (filters.LawyerId.HasValue)
        {
            rows = rows.Where(x => x.LawyerId == filters.LawyerId.Value).ToList();
        }

        return rows;
    }

    private async Task<ReportDistributionVM> BuildDistributionAsync(ReportFiltersVM filters)
    {
        var query = BuildCaseQuery(filters);
        var cases = await query.ToListAsync();

        return new ReportDistributionVM
        {
            Filters = filters,
            ByType = cases.GroupBy(x => new { x.CaseTypeId, Name = x.CaseType?.NameAr ?? "بدون نوع" })
                .Select(g => new ReportTypeRowVM { TypeId = g.Key.CaseTypeId, TypeName = g.Key.Name, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.TypeName)
                .ToList(),
            ByStatus = cases.GroupBy(x => new { x.CaseStatusId, Name = x.CaseStatus?.NameAr ?? "بدون حالة" })
                .Select(g => new ReportStatusRowVM { StatusId = g.Key.CaseStatusId, StatusName = g.Key.Name, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.StatusName)
                .ToList(),
            ByCourt = cases.GroupBy(x => x.Court?.NameAr ?? "بدون محكمة")
                .Select(g => new ReportCountRowVM { Label = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Label)
                .ToList()
        };
    }

    private async Task FillFiltersAsync(ReportFiltersVM filters)
    {
        filters.Courts = await _db.Lookups.AsNoTracking()
            .Where(x => x.Type == "Court" && x.IsActive)
            .OrderBy(x => x.NameAr)
            .Select(x => new SelectListItem(x.NameAr, x.Id.ToString()))
            .ToListAsync();

        filters.Lawyers = await _db.Lawyers.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.FullName)
            .Select(x => new SelectListItem(x.FullName, x.Id.ToString()))
            .ToListAsync();

        filters.CaseTypes = await _caseTypeOptions.GetVisibleCaseTypesAsync(User);

        filters.CaseStatuses = await _db.Lookups.AsNoTracking()
            .Where(x => x.Type == "CaseStatus" && x.IsActive)
            .OrderBy(x => x.NameAr)
            .Select(x => new SelectListItem(x.NameAr, x.Id.ToString()))
            .ToListAsync();
    }

    private byte[] BuildSummaryExcel(ReportSummaryVM vm)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("ملخص التقارير");
        ws.Cell(1, 1).Value = "تقرير عام";
        ws.Cell(3, 1).Value = "إجمالي القضايا";
        ws.Cell(3, 2).Value = vm.TotalCases;
        ws.Cell(4, 1).Value = "القضايا المفتوحة";
        ws.Cell(4, 2).Value = vm.OpenCases;
        ws.Cell(5, 1).Value = "القضايا المغلقة";
        ws.Cell(5, 2).Value = vm.ClosedCases;
        ws.Cell(6, 1).Value = "إجمالي الأتعاب";
        ws.Cell(6, 2).Value = vm.TotalFees;
        ws.Cell(7, 1).Value = "إجمالي المسدد";
        ws.Cell(7, 2).Value = vm.TotalPaid;
        ws.Cell(8, 1).Value = "المتبقي";
        ws.Cell(8, 2).Value = vm.TotalRemaining;
        return SaveWorkbook(workbook);
    }

    private byte[] BuildLawyersExcel(ReportLawyerVM vm)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("تقارير المحامين");
        ws.Cell(1, 1).Value = "المحامي";
        ws.Cell(1, 2).Value = "إجمالي القضايا";
        ws.Cell(1, 3).Value = "المفتوحة";
        ws.Cell(1, 4).Value = "المغلقة";
        var row = 2;
        foreach (var item in vm.Rows)
        {
            ws.Cell(row, 1).Value = item.LawyerName;
            ws.Cell(row, 2).Value = item.CasesCount;
            ws.Cell(row, 3).Value = item.OpenCases;
            ws.Cell(row, 4).Value = item.ClosedCases;
            row++;
        }
        return SaveWorkbook(workbook);
    }

    private byte[] BuildDistributionExcel(ReportDistributionVM vm)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("توزيع القضايا");
        ws.Cell(1, 1).Value = "النوع";
        ws.Cell(1, 2).Value = "العدد";
        var row = 2;
        foreach (var item in vm.ByType)
        {
            ws.Cell(row, 1).Value = item.TypeName;
            ws.Cell(row, 2).Value = item.Count;
            row++;
        }
        return SaveWorkbook(workbook);
    }

    private byte[] BuildSummaryPdf(ReportSummaryVM vm) => new SummaryPdf(vm).GeneratePdf();
    private byte[] BuildLawyersPdf(ReportLawyerVM vm) => new LawyersPdf(vm).GeneratePdf();
    private byte[] BuildDistributionPdf(ReportDistributionVM vm) => new DistributionPdf(vm).GeneratePdf();

    private static byte[] SaveWorkbook(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private sealed class SummaryPdf : IDocument
    {
        private readonly ReportSummaryVM _vm;
        public SummaryPdf(ReportSummaryVM vm) => _vm = vm;
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Content().Column(col =>
                {
                    col.Item().Text("تقرير عام").Bold().FontSize(18);
                    col.Item().Text($"إجمالي القضايا: {_vm.TotalCases}");
                    col.Item().Text($"المفتوحة: {_vm.OpenCases} - المغلقة: {_vm.ClosedCases}");
                    col.Item().Text($"الأتعاب: {_vm.TotalFees} - المسدد: {_vm.TotalPaid} - المتبقي: {_vm.TotalRemaining}");
                });
            });
        }
    }

    private sealed class LawyersPdf : IDocument
    {
        private readonly ReportLawyerVM _vm;
        public LawyersPdf(ReportLawyerVM vm) => _vm = vm;
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Content().Column(col =>
                {
                    col.Item().Text("تقارير المحامين").Bold().FontSize(18);
                    foreach (var row in _vm.Rows)
                    {
                        col.Item().Text($"{row.LawyerName}: {row.CasesCount} قضية");
                    }
                });
            });
        }
    }

    private sealed class DistributionPdf : IDocument
    {
        private readonly ReportDistributionVM _vm;
        public DistributionPdf(ReportDistributionVM vm) => _vm = vm;
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Content().Column(col =>
                {
                    col.Item().Text("توزيع القضايا").Bold().FontSize(18);
                    foreach (var item in _vm.ByType)
                    {
                        col.Item().Text($"{item.TypeName}: {item.Count}");
                    }
                });
            });
        }
    }
}
