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

        var cases = await BuildCaseQuery(filters).ToListAsync();
        var rows = BuildLawyerRows(cases, filters);
        var vm = new ReportLawyerVM
        {
            Filters = filters,
            Rows = rows,
            Cases = BuildCaseRows(cases),
            TotalCases = cases.Count
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

    public async Task<IActionResult> CaseReport(int id)
    {
        if (!await EnsureAccessAsync("Cases.View")) return Forbid();

        var vm = await BuildCaseReportAsync(id);
        if (vm == null)
        {
            return NotFound();
        }

        return File(BuildCaseReportPdf(vm), "application/pdf", MakeSafeFileName($"LegalOffice-Case-{vm.CaseNumber}.pdf"));
    }

    public async Task<IActionResult> ConsultationReport(int id)
    {
        if (!await EnsureAccessAsync("Consultations.View")) return Forbid();

        var vm = await BuildConsultationReportAsync(id);
        if (vm == null)
        {
            return NotFound();
        }

        return File(BuildConsultationReportPdf(vm), "application/pdf", MakeSafeFileName($"LegalOffice-Consultation-{vm.ConsultationNumber}.pdf"));
    }

    private async Task<bool> EnsureAccessAsync(string code) => await _permissions.HasPermissionAsync(User, code);

    private async Task<CaseReportVM?> BuildCaseReportAsync(int id)
    {
        var query = _db.Cases
            .AsNoTracking()
            .Include(x => x.Client)
            .Include(x => x.CaseType)
            .Include(x => x.CaseStatus)
            .Include(x => x.Court)
            .Include(x => x.Priority)
            .Include(x => x.CreatedByUser)
            .Include(x => x.CaseLawyers).ThenInclude(x => x.Lawyer)
            .Include(x => x.CaseLawyers).ThenInclude(x => x.AccessLevel)
            .Include(x => x.Hearings).ThenInclude(x => x.HearingStatus)
            .Include(x => x.Documents).ThenInclude(x => x.DocumentType)
            .Include(x => x.Timelines)
            .Include(x => x.Expenses).ThenInclude(x => x.ExpenseType)
            .Include(x => x.Expenses).ThenInclude(x => x.StatusLookup)
            .Include(x => x.Expenses).ThenInclude(x => x.SubmittedByUser)
            .Include(x => x.Expenses).ThenInclude(x => x.ApprovedByUser)
            .Include(x => x.ConflictChecks).ThenInclude(x => x.ResultStatusLookup)
            .Include(x => x.ConflictChecks).ThenInclude(x => x.CheckedByUser)
            .Include(x => x.Payments).ThenInclude(x => x.PaymentStatus)
            .Include(x => x.Payments).ThenInclude(x => x.PaymentMethod)
            .AsQueryable();

        if (!await _permissions.HasPermissionAsync(User, "Cases.Edit"))
        {
            var lawyerId = await GetCurrentLawyerIdAsync();
            if (!lawyerId.HasValue)
            {
                return null;
            }

            query = query.Where(x => x.CaseLawyers.Any(cl => cl.LawyerId == lawyerId.Value));
        }

        var entity = await query.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return null;
        }

        var assignmentHistory = await _db.CaseAssignmentHistories
            .AsNoTracking()
            .Where(x => x.CaseId == id)
            .Include(x => x.Lawyer)
            .Include(x => x.ChangedByUser)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        var stageHistory = await _db.CaseStageHistories
            .AsNoTracking()
            .Where(x => x.CaseId == id)
            .Include(x => x.FromStageLookup)
            .Include(x => x.ToStageLookup)
            .Include(x => x.ChangedByUser)
            .OrderByDescending(x => x.ChangedAt)
            .ToListAsync();

        var internalNotes = await _db.CaseInternalNotes
            .AsNoTracking()
            .Where(x => x.CaseId == id && !x.IsDeleted)
            .Include(x => x.CreatedByUser)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        var messageLogs = await _db.MessageLogs
            .AsNoTracking()
            .Where(x => x.CaseId == id)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        var totalPaid = entity.Payments
            .Where(x => string.Equals(x.PaymentStatus?.NameEn, "Received", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(x.PaymentStatus?.NameAr, "مستلم", StringComparison.OrdinalIgnoreCase))
            .Sum(x => x.Amount);

        var vm = new CaseReportVM
        {
            Id = entity.Id,
            CaseNumber = entity.CaseNumber,
            Title = entity.Title,
            Description = entity.Description,
            ClientName = entity.Client.FullName,
            CaseType = entity.CaseType.NameAr,
            CaseStatus = entity.CaseStatus.NameAr,
            Court = entity.Court?.NameAr,
            Circuit = entity.Circuit,
            OpponentName = entity.OpponentName,
            OpponentLawyer = entity.OpponentLawyer,
            Priority = entity.Priority.NameAr,
            CreatedBy = entity.CreatedByUser?.FullName ?? entity.CreatedByUser?.UserName,
            StartDate = entity.StartDate,
            ClosedDate = entity.ClosedDate,
            CaseYear = entity.CaseYear,
            FeesAmount = entity.FeesAmount,
            TotalPaid = totalPaid,
            Remaining = Math.Max(entity.FeesAmount - totalPaid, 0),
            CreatedAt = entity.CreatedAt,
            LastStageChangedAt = entity.LastStageChangedAt,
            Lawyers = entity.CaseLawyers
                .OrderByDescending(x => x.IsMainLawyer)
                .ThenBy(x => x.Lawyer.FullName)
                .Select(x => new CaseReportLawyerRowVM
                {
                    LawyerName = x.Lawyer.FullName,
                    Role = x.IsMainLawyer ? "رئيسي" : x.RoleInCase ?? "مشارك",
                    AccessLevel = x.AccessLevel?.NameAr ?? "-"
                })
                .ToList(),
            Hearings = entity.Hearings
                .OrderByDescending(x => x.HearingDate)
                .Select(x => new CaseReportHearingRowVM
                {
                    HearingDate = x.HearingDate,
                    HearingStatus = x.HearingStatus.NameAr,
                    CourtDecision = x.CourtDecision,
                    NextHearingDate = x.NextHearingDate,
                    NextRequirements = x.NextRequirements,
                    Notes = x.Notes
                })
                .ToList(),
            Payments = entity.Payments
                .OrderByDescending(x => x.PaymentDate)
                .Select(x => new CaseReportPaymentRowVM
                {
                    PaymentDate = x.PaymentDate,
                    Amount = x.Amount,
                    Status = x.PaymentStatus.NameAr,
                    Method = x.PaymentMethod?.NameAr,
                    ReferenceNumber = x.ReferenceNumber,
                    Notes = x.Notes
                })
                .ToList(),
            Documents = entity.Documents
                .OrderByDescending(x => x.UploadedAt)
                .Select(x => new CaseReportDocumentRowVM
                {
                    UploadedAt = x.UploadedAt,
                    DocumentType = x.DocumentType.NameAr,
                    FileName = x.FileName,
                    Notes = x.Notes
                })
                .ToList(),
            Messages = messageLogs
                .Select(x => new CaseReportMessageRowVM
                {
                    CreatedAt = x.CreatedAt,
                    Channel = x.Channel,
                    PhoneNumber = x.PhoneNumber,
                    IsSent = x.IsSent,
                    MessageText = x.MessageText,
                    ProviderResponse = x.ProviderResponse
                })
                .ToList(),
            Activity = BuildCaseActivityTimeline(entity, stageHistory, assignmentHistory, internalNotes, messageLogs)
        };

        return vm;
    }

    private async Task<ConsultationReportVM?> BuildConsultationReportAsync(int id)
    {
        var entity = await _db.LegalConsultations
            .AsNoTracking()
            .Include(x => x.Client)
            .Include(x => x.AssignedLawyer)
            .Include(x => x.ConsultationTypeLookup)
            .Include(x => x.ConsultationStatusLookup)
            .Include(x => x.Branch)
            .Include(x => x.Department)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
        {
            return null;
        }

        return new ConsultationReportVM
        {
            Id = entity.Id,
            ConsultationNumber = entity.ConsultationNumber,
            Title = entity.Title,
            ClientName = entity.Client.FullName,
            AssignedLawyer = entity.AssignedLawyer.FullName,
            ConsultationType = entity.ConsultationTypeLookup.NameAr,
            ConsultationStatus = entity.ConsultationStatusLookup.NameAr,
            Branch = entity.Branch?.NameAr,
            Department = entity.Department?.NameAr,
            RequestDate = entity.RequestDate,
            ResponseDate = entity.ResponseDate,
            ConsultationFees = entity.ConsultationFees,
            Subject = entity.Subject,
            LegalOpinion = entity.LegalOpinion,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Activity = BuildConsultationActivityTimeline(entity)
        };
    }

    private async Task<int?> GetCurrentLawyerIdAsync()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        return await _db.Lawyers
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync();
    }

    private static List<CaseReportActivityVM> BuildCaseActivityTimeline(
        LegalCase entity,
        IReadOnlyCollection<CaseStageHistory> stageHistory,
        IReadOnlyCollection<CaseAssignmentHistory> assignmentHistory,
        IReadOnlyCollection<CaseInternalNote> internalNotes,
        IReadOnlyCollection<MessageLog> messageLogs)
    {
        var items = new List<CaseReportActivityVM>();

        items.Add(new CaseReportActivityVM
        {
            Date = entity.CreatedAt,
            Category = "إنشاء",
            Title = "تم إنشاء القضية",
            Description = $"{entity.CaseNumber} - {entity.Title}"
        });

        if (entity.LastStageChangedAt.HasValue)
        {
            items.Add(new CaseReportActivityVM
            {
                Date = entity.LastStageChangedAt.Value,
                Category = "تحديث مرحلي",
                Title = "آخر تغيير في مرحلة القضية",
                Description = entity.WorkflowStageLookup?.NameAr ?? "-"
            });
        }

        if (entity.ClosedDate.HasValue)
        {
            items.Add(new CaseReportActivityVM
            {
                Date = entity.ClosedDate.Value,
                Category = "إغلاق",
                Title = "تم إغلاق القضية",
                Description = entity.CaseStatus?.NameAr ?? "-"
            });
        }

        items.AddRange(entity.Timelines.Select(x => new CaseReportActivityVM
        {
            Date = x.CreatedAt,
            Category = "مراحل القضية",
            Title = x.Title,
            Description = x.Description
        }));

        items.AddRange(stageHistory.Select(x => new CaseReportActivityVM
        {
            Date = x.ChangedAt,
            Category = "تغيير مرحلة",
            Title = $"{x.FromStageLookup?.NameAr ?? "بداية"} → {x.ToStageLookup.NameAr}",
            Description = x.Notes
        }));

        items.AddRange(assignmentHistory.Select(x => new CaseReportActivityVM
        {
            Date = x.CreatedAt,
            Category = "إسناد محامٍ",
            Title = x.ActionType,
            Description = $"{x.Lawyer.FullName}{(string.IsNullOrWhiteSpace(x.Notes) ? string.Empty : $" - {x.Notes}")}"
        }));

        items.AddRange(entity.Hearings.Select(x => new CaseReportActivityVM
        {
            Date = x.HearingDate,
            Category = "جلسة",
            Title = x.HearingStatus.NameAr,
            Description = $"{x.CourtDecision ?? "لا يوجد قرار مسجل"}{(x.NextHearingDate.HasValue ? $" | الجلسة القادمة: {x.NextHearingDate:yyyy/MM/dd}" : string.Empty)}"
        }));

        items.AddRange(internalNotes.Select(x => new CaseReportActivityVM
        {
            Date = x.CreatedAt,
            Category = "ملاحظة داخلية",
            Title = x.Note,
            Description = x.CreatedByUser?.FullName
        }));

        items.AddRange(entity.Documents.Select(x => new CaseReportActivityVM
        {
            Date = x.UploadedAt,
            Category = "مستند",
            Title = x.DocumentType.NameAr,
            Description = x.FileName
        }));

        items.AddRange(entity.Payments.Select(x => new CaseReportActivityVM
        {
            Date = x.PaymentDate,
            Category = "دفعة",
            Title = x.PaymentStatus.NameAr,
            Description = $"{x.Amount:N2}{(string.IsNullOrWhiteSpace(x.ReferenceNumber) ? string.Empty : $" | {x.ReferenceNumber}")}"
        }));

        items.AddRange(entity.Expenses.Select(x => new CaseReportActivityVM
        {
            Date = x.ExpenseDate,
            Category = "مصروف",
            Title = x.ExpenseType?.NameAr ?? "مصروف",
            Description = $"{x.Amount:N2}{(string.IsNullOrWhiteSpace(x.Notes) ? string.Empty : $" | {x.Notes}")}"
        }));

        items.AddRange(entity.ConflictChecks.Select(x => new CaseReportActivityVM
        {
            Date = x.CreatedAt,
            Category = "فحص تعارض",
            Title = x.ResultStatusLookup.NameAr,
            Description = $"{x.CheckedByUser?.FullName ?? x.CheckedByUserId}{(string.IsNullOrWhiteSpace(x.Notes) ? string.Empty : $" | {x.Notes}")}"
        }));

        items.AddRange(messageLogs.Select(x => new CaseReportActivityVM
        {
            Date = x.CreatedAt,
            Category = "رسالة",
            Title = $"{x.Channel} - {(x.IsSent ? "مرسلة" : "غير مرسلة")}",
            Description = ShortenText(x.MessageText, 220)
        }));

        return items
            .OrderByDescending(x => x.Date)
            .ToList();
    }

    private static List<ConsultationReportActivityVM> BuildConsultationActivityTimeline(LegalConsultation entity)
    {
        var items = new List<ConsultationReportActivityVM>
        {
            new()
            {
                Date = entity.CreatedAt,
                Category = "إنشاء",
                Title = "تم إنشاء الاستشارة",
                Description = entity.CreatedAt.ToString("yyyy/MM/dd HH:mm")
            },
            new()
            {
                Date = entity.RequestDate,
                Category = "طلب",
                Title = "تاريخ الطلب",
                Description = entity.Subject
            }
        };

        if (entity.ResponseDate.HasValue)
        {
            items.Add(new ConsultationReportActivityVM
            {
                Date = entity.ResponseDate.Value,
                Category = "رد",
                Title = "تاريخ الرد",
                Description = entity.LegalOpinion
            });
        }

        if (entity.UpdatedAt.HasValue)
        {
            items.Add(new ConsultationReportActivityVM
            {
                Date = entity.UpdatedAt.Value,
                Category = "تعديل",
                Title = "آخر تعديل",
                Description = entity.Notes
            });
        }

        return items
            .OrderByDescending(x => x.Date)
            .ToList();
    }

    private static string ShortenText(string? text, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "-";
        }

        text = text.Trim();
        return text.Length <= maxLength ? text : text[..maxLength] + "...";
    }

    private static string MakeSafeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var safe = new string(fileName.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
        return string.IsNullOrWhiteSpace(safe) ? "report.pdf" : safe;
    }

    private byte[] BuildCaseReportPdf(CaseReportVM vm) => new CaseDetailPdf(vm).GeneratePdf();
    private byte[] BuildConsultationReportPdf(ConsultationReportVM vm) => new ConsultationDetailPdf(vm).GeneratePdf();

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
            Cases = BuildCaseRows(cases),
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

    private List<ReportLawyerRowVM> BuildLawyerRows(IReadOnlyCollection<LegalCase> cases, ReportFiltersVM filters)
    {
        var rows = cases
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
            .ToList();

        if (filters.LawyerId.HasValue)
        {
            rows = rows.Where(x => x.LawyerId == filters.LawyerId.Value).ToList();
        }

        return rows;
    }

    private async Task<ReportDistributionVM> BuildDistributionAsync(ReportFiltersVM filters)
    {
        var cases = await BuildCaseQuery(filters).ToListAsync();

        return new ReportDistributionVM
        {
            Filters = filters,
            Cases = BuildCaseRows(cases),
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

    private List<ReportCaseRowVM> BuildCaseRows(IEnumerable<LegalCase> cases)
    {
        return cases.Select(c => new ReportCaseRowVM
        {
            Id = c.Id,
            CaseNumber = c.CaseNumber,
            CaseYear = c.CaseYear,
            CaseType = c.CaseType?.NameAr ?? "بدون نوع",
            Court = c.Court?.NameAr ?? "بدون محكمة",
            Status = c.CaseStatus?.NameAr ?? "بدون حالة",
            Lawyers = string.Join("، ", c.CaseLawyers
                .Select(cl => cl.Lawyer.FullName)
                .Distinct()
                .OrderBy(x => x)),
            StartDate = c.StartDate,
            FeesAmount = c.FeesAmount,
            TotalPaid = c.Payments.Where(p => p.PaymentStatusId > 0).Sum(p => p.Amount)
        }).ToList();
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
        var ws = workbook.Worksheets.Add("ملخص التقرير");
        InitializeWorksheet(ws);

        int row = WriteWorkbookHeader(ws, "تقرير عام", "ملخص شامل للقضايا مع الفلاتر المختارة", vm.Filters);
        row = WriteKpiTable(ws, row, new[]
        {
            ("إجمالي القضايا", vm.TotalCases.ToString()),
            ("القضايا المفتوحة", vm.OpenCases.ToString()),
            ("القضايا المغلقة", vm.ClosedCases.ToString()),
            ("إجمالي الأتعاب", vm.TotalFees.ToString("N2")),
            ("إجمالي المسدد", vm.TotalPaid.ToString("N2")),
            ("المتبقي", vm.TotalRemaining.ToString("N2"))
        });

        row += 1;
        row = WriteCountSection(ws, row, "التوزيع حسب المحكمة", "المحكمة", vm.ByCourt);
        row += 1;
        row = WriteCountSection(ws, row, "التوزيع حسب نوع القضية", "النوع", vm.ByType);
        row += 1;
        row = WriteCountSection(ws, row, "التوزيع حسب الحالة", "الحالة", vm.ByStatus);
        row += 2;
        WriteCaseDetailsSheet(workbook, "تفاصيل القضايا", vm.Filters, vm.Cases);

        AutoFitAndFinish(ws);
        return SaveWorkbook(workbook);
    }

    private byte[] BuildLawyersExcel(ReportLawyerVM vm)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("تقارير المحامين");
        InitializeWorksheet(ws);

        int row = WriteWorkbookHeader(ws, "تقرير المحامين", "عدد القضايا لكل محامٍ مع التفاصيل المرتبطة", vm.Filters);
        row = WriteKpiTable(ws, row, new[]
        {
            ("إجمالي القضايا", vm.TotalCases.ToString()),
            ("عدد المحامين", vm.Rows.Count.ToString())
        });

        row += 1;
        row = WriteLawyersTable(ws, row, vm.Rows);
        row += 2;
        WriteCaseDetailsSheet(workbook, "تفاصيل القضايا", vm.Filters, vm.Cases);

        AutoFitAndFinish(ws);
        return SaveWorkbook(workbook);
    }

    private byte[] BuildDistributionExcel(ReportDistributionVM vm)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("توزيع القضايا");
        InitializeWorksheet(ws);

        int row = WriteWorkbookHeader(ws, "توزيع القضايا", "توزيع القضايا حسب النوع والحالة والمحكمة", vm.Filters);
        row = WriteCountSection(ws, row, "حسب النوع", "النوع", vm.ByType.Select(x => new ReportCountRowVM { Label = x.TypeName, Count = x.Count }).ToList());
        row += 1;
        row = WriteCountSection(ws, row, "حسب الحالة", "الحالة", vm.ByStatus.Select(x => new ReportCountRowVM { Label = x.StatusName, Count = x.Count }).ToList());
        row += 1;
        row = WriteCountSection(ws, row, "حسب المحكمة", "المحكمة", vm.ByCourt);
        row += 2;
        WriteCaseDetailsSheet(workbook, "تفاصيل القضايا", vm.Filters, vm.Cases);

        AutoFitAndFinish(ws);
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

    private static void InitializeWorksheet(IXLWorksheet ws)
    {
        ws.RightToLeft = true;
        ws.Style.Font.FontName = "Cairo";
        ws.Style.Font.FontSize = 11;
        ws.SheetView.FreezeRows(4);
    }

    private int WriteWorkbookHeader(IXLWorksheet ws, string title, string subtitle, ReportFiltersVM filters)
    {
        ws.Cell(1, 1).Value = title;
        ws.Range(1, 1, 1, 10).Merge();
        ws.Range(1, 1, 1, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#3F2D19");
        ws.Range(1, 1, 1, 10).Style.Font.FontColor = XLColor.White;
        ws.Range(1, 1, 1, 10).Style.Font.Bold = true;
        ws.Range(1, 1, 1, 10).Style.Font.FontSize = 16;

        ws.Cell(2, 1).Value = subtitle;
        ws.Range(2, 1, 2, 10).Merge();
        ws.Range(2, 1, 2, 10).Style.Font.FontColor = XLColor.FromHtml("#6B5F53");
        ws.Range(2, 1, 2, 10).Style.Font.Italic = true;

        ws.Cell(4, 1).Value = "الفلترة المختارة";
        ws.Range(4, 1, 4, 10).Merge();
        ws.Range(4, 1, 4, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2E9D8");
        ws.Range(4, 1, 4, 10).Style.Font.Bold = true;

        ws.Cell(5, 1).Value = "المحكمة";
        ws.Cell(5, 2).Value = GetSelectedText(filters.Courts, filters.CourtId);
        ws.Cell(5, 4).Value = "المحامي";
        ws.Cell(5, 5).Value = GetSelectedText(filters.Lawyers, filters.LawyerId);

        ws.Cell(6, 1).Value = "نوع القضية";
        ws.Cell(6, 2).Value = GetSelectedText(filters.CaseTypes, filters.CaseTypeId);
        ws.Cell(6, 4).Value = "حالة القضية";
        ws.Cell(6, 5).Value = GetSelectedText(filters.CaseStatuses, filters.CaseStatusId);

        ws.Cell(7, 1).Value = "من تاريخ";
        ws.Cell(7, 2).Value = filters.FromDate?.ToString("yyyy/MM/dd") ?? "الكل";
        ws.Cell(7, 4).Value = "إلى تاريخ";
        ws.Cell(7, 5).Value = filters.ToDate?.ToString("yyyy/MM/dd") ?? "الكل";

        ws.Cell(8, 1).Value = "تاريخ التوليد";
        ws.Cell(8, 2).Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        ws.Range(5, 1, 8, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Range(5, 1, 8, 5).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        return 10;
    }

    private static string GetSelectedText(IEnumerable<SelectListItem> items, int? value)
    {
        if (!value.HasValue) return "الكل";
        return items.FirstOrDefault(x => x.Value == value.Value.ToString())?.Text ?? value.Value.ToString();
    }

    private int WriteKpiTable(IXLWorksheet ws, int startRow, IReadOnlyList<(string Label, string Value)> items)
    {
        var col = 1;
        var row = startRow;
        foreach (var item in items)
        {
            ws.Cell(row, col).Value = item.Label;
            ws.Cell(row, col + 1).Value = item.Value;
            ws.Range(row, col, row, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range(row, col, row, col + 1).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2E9D8");
            ws.Cell(row, col).Style.Font.Bold = true;
            col += 3;
            if (col > 7)
            {
                col = 1;
                row++;
            }
        }

        return row + 2;
    }

    private int WriteCountSection(IXLWorksheet ws, int startRow, string sectionTitle, string firstColumnHeader, IReadOnlyList<ReportCountRowVM> rows)
    {
        ws.Cell(startRow, 1).Value = sectionTitle;
        ws.Range(startRow, 1, startRow, 4).Merge();
        ws.Range(startRow, 1, startRow, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#6B4F2A");
        ws.Range(startRow, 1, startRow, 4).Style.Font.FontColor = XLColor.White;
        ws.Range(startRow, 1, startRow, 4).Style.Font.Bold = true;

        var headerRow = startRow + 1;
        ws.Cell(headerRow, 1).Value = firstColumnHeader;
        ws.Cell(headerRow, 2).Value = "العدد";
        ws.Range(headerRow, 1, headerRow, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#E9DCC7");
        ws.Range(headerRow, 1, headerRow, 2).Style.Font.Bold = true;

        var rowIndex = headerRow + 1;
        foreach (var item in rows)
        {
            ws.Cell(rowIndex, 1).Value = item.Label;
            ws.Cell(rowIndex, 2).Value = item.Count;
            rowIndex++;
        }

        ws.Range(headerRow, 1, Math.Max(headerRow, rowIndex - 1), 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Range(headerRow, 1, Math.Max(headerRow, rowIndex - 1), 2).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        return rowIndex + 1;
    }

    private int WriteLawyersTable(IXLWorksheet ws, int startRow, IReadOnlyList<ReportLawyerRowVM> rows)
    {
        ws.Cell(startRow, 1).Value = "المحامي";
        ws.Cell(startRow, 2).Value = "إجمالي القضايا";
        ws.Cell(startRow, 3).Value = "المفتوحة";
        ws.Cell(startRow, 4).Value = "المغلقة";
        ws.Range(startRow, 1, startRow, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#E9DCC7");
        ws.Range(startRow, 1, startRow, 4).Style.Font.Bold = true;

        var row = startRow + 1;
        foreach (var item in rows)
        {
            ws.Cell(row, 1).Value = item.LawyerName;
            ws.Cell(row, 2).Value = item.CasesCount;
            ws.Cell(row, 3).Value = item.OpenCases;
            ws.Cell(row, 4).Value = item.ClosedCases;
            row++;
        }

        ws.Range(startRow, 1, Math.Max(startRow, row - 1), 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Range(startRow, 1, Math.Max(startRow, row - 1), 4).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        return row + 1;
    }

    private void WriteCaseDetailsSheet(XLWorkbook workbook, string sheetName, ReportFiltersVM filters, IReadOnlyList<ReportCaseRowVM> cases)
    {
        var ws = workbook.Worksheets.Add(sheetName);
        InitializeWorksheet(ws);

        int row = WriteWorkbookHeader(ws, "تفاصيل القضايا", "بيانات تفصيلية للقضايا المطابقة للفلاتر المختارة", filters);

        ws.Cell(row, 1).Value = "رقم القضية";
        ws.Cell(row, 2).Value = "السنة";
        ws.Cell(row, 3).Value = "نوع القضية";
        ws.Cell(row, 4).Value = "المحكمة";
        ws.Cell(row, 5).Value = "الحالة";
        ws.Cell(row, 6).Value = "المحامي";
        ws.Cell(row, 7).Value = "تاريخ البداية";
        ws.Cell(row, 8).Value = "الأتعاب";
        ws.Cell(row, 9).Value = "المسدد";
        ws.Cell(row, 10).Value = "المتبقي";
        ws.Range(row, 1, row, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#E9DCC7");
        ws.Range(row, 1, row, 10).Style.Font.Bold = true;

        var dataRow = row + 1;
        foreach (var item in cases)
        {
            ws.Cell(dataRow, 1).Value = item.CaseNumber;
            ws.Cell(dataRow, 2).Value = item.CaseYear;
            ws.Cell(dataRow, 3).Value = item.CaseType;
            ws.Cell(dataRow, 4).Value = item.Court;
            ws.Cell(dataRow, 5).Value = item.Status;
            ws.Cell(dataRow, 6).Value = item.Lawyers;
            ws.Cell(dataRow, 7).Value = item.StartDate;
            ws.Cell(dataRow, 8).Value = item.FeesAmount;
            ws.Cell(dataRow, 9).Value = item.TotalPaid;
            ws.Cell(dataRow, 10).Value = item.Remaining;
            dataRow++;
        }

        ws.Range(row, 1, Math.Max(row, dataRow - 1), 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Range(row, 1, Math.Max(row, dataRow - 1), 10).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        ws.Columns().AdjustToContents();
    }

    private static void AutoFitAndFinish(IXLWorksheet ws)
    {
        ws.Columns().AdjustToContents();
        ws.Rows().AdjustToContents();
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
                page.Margin(24);
                page.DefaultTextStyle(x => x.FontFamily("Cairo").FontSize(10));

                page.Header().Element(c => RenderPdfHeader(c, "تقرير عام", "ملخص شامل للقضايا مع الفلاتر المختارة", _vm.Filters));
                page.Content().Column(col =>
                {
                    col.Spacing(10);
                    col.Item().Element(c => RenderPdfKpis(c, new[]
                    {
                        ("إجمالي القضايا", _vm.TotalCases.ToString()),
                        ("القضايا المفتوحة", _vm.OpenCases.ToString()),
                        ("القضايا المغلقة", _vm.ClosedCases.ToString()),
                        ("إجمالي الأتعاب", _vm.TotalFees.ToString("N2")),
                        ("إجمالي المسدد", _vm.TotalPaid.ToString("N2")),
                        ("المتبقي", _vm.TotalRemaining.ToString("N2"))
                    }));
                    col.Item().Element(c => RenderPdfCountTable(c, "التوزيع حسب المحكمة", "المحكمة", _vm.ByCourt));
                    col.Item().Element(c => RenderPdfCountTable(c, "التوزيع حسب نوع القضية", "النوع", _vm.ByType));
                    col.Item().Element(c => RenderPdfCountTable(c, "التوزيع حسب الحالة", "الحالة", _vm.ByStatus));
                    col.Item().Element(c => RenderPdfCases(c, "تفاصيل القضايا", _vm.Cases));
                });

                page.Footer().AlignCenter().Text($"تم التوليد في {DateTime.Now:yyyy/MM/dd HH:mm}");
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
                page.Margin(24);
                page.DefaultTextStyle(x => x.FontFamily("Cairo").FontSize(10));

                page.Header().Element(c => RenderPdfHeader(c, "تقرير المحامين", "عدد القضايا لكل محامٍ مع التفاصيل المرتبطة", _vm.Filters));
                page.Content().Column(col =>
                {
                    col.Spacing(10);
                    col.Item().Element(c => RenderPdfKpis(c, new[]
                    {
                        ("إجمالي القضايا", _vm.TotalCases.ToString()),
                        ("عدد المحامين", _vm.Rows.Count.ToString())
                    }));
                    col.Item().Element(c => RenderPdfLawyersTable(c, _vm.Rows));
                    col.Item().Element(c => RenderPdfCases(c, "تفاصيل القضايا", _vm.Cases));
                });

                page.Footer().AlignCenter().Text($"تم التوليد في {DateTime.Now:yyyy/MM/dd HH:mm}");
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
                page.Margin(24);
                page.DefaultTextStyle(x => x.FontFamily("Cairo").FontSize(10));

                page.Header().Element(c => RenderPdfHeader(c, "توزيع القضايا", "توزيع القضايا حسب النوع والحالة والمحكمة", _vm.Filters));
                page.Content().Column(col =>
                {
                    col.Spacing(10);
                    col.Item().Element(c => RenderPdfCountTable(c, "حسب النوع", "النوع", _vm.ByType.Select(x => new ReportCountRowVM { Label = x.TypeName, Count = x.Count }).ToList()));
                    col.Item().Element(c => RenderPdfCountTable(c, "حسب الحالة", "الحالة", _vm.ByStatus.Select(x => new ReportCountRowVM { Label = x.StatusName, Count = x.Count }).ToList()));
                    col.Item().Element(c => RenderPdfCountTable(c, "حسب المحكمة", "المحكمة", _vm.ByCourt));
                    col.Item().Element(c => RenderPdfCases(c, "تفاصيل القضايا", _vm.Cases));
                });

                page.Footer().AlignCenter().Text($"تم التوليد في {DateTime.Now:yyyy/MM/dd HH:mm}");
            });
        }
    }

    private static void RenderPdfHeader(IContainer container, string title, string subtitle, ReportFiltersVM filters)
    {
        container.Column(col =>
        {
            col.Spacing(4);
            col.Item().Text(title).Bold().FontSize(18).FontColor(Colors.Brown.Darken4);
            col.Item().Text(subtitle).FontSize(10).FontColor(Colors.Grey.Darken1);
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                AddPdfFilterCell(table, "المحكمة", GetSelectedText(filters.Courts, filters.CourtId));
                AddPdfFilterCell(table, "المحامي", GetSelectedText(filters.Lawyers, filters.LawyerId));
                AddPdfFilterCell(table, "نوع القضية", GetSelectedText(filters.CaseTypes, filters.CaseTypeId));
                AddPdfFilterCell(table, "حالة القضية", GetSelectedText(filters.CaseStatuses, filters.CaseStatusId));
            });
        });
    }

    private static void AddPdfFilterCell(TableDescriptor table, string label, string value)
    {
        table.Cell().Element(CellHeader).Text(label);
        table.Cell().Element(CellBody).Text(value);
    }

    private static void RenderPdfKpis(IContainer container, IReadOnlyList<(string Label, string Value)> items)
    {
        RenderFactList(container, null, items);
    }

    private static void RenderPdfCountTable(IContainer container, string title, string firstColumnHeader, IReadOnlyList<ReportCountRowVM> rows)
    {
        container.Column(col =>
        {
            col.Spacing(4);
            col.Item().Text(title).Bold().FontSize(13).FontColor(Colors.Brown.Darken3);
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellHeader).Text(firstColumnHeader);
                    header.Cell().Element(CellHeader).AlignCenter().Text("العدد");
                });

                foreach (var row in rows)
                {
                    table.Cell().Element(CellBody).Text(row.Label);
                    table.Cell().Element(CellBody).AlignCenter().Text(row.Count.ToString());
                }
            });
        });
    }

    private static void RenderPdfLawyersTable(IContainer container, IReadOnlyList<ReportLawyerRowVM> rows)
    {
        container.Column(col =>
        {
            col.Spacing(4);
            col.Item().Text("تفاصيل المحامين").Bold().FontSize(13).FontColor(Colors.Brown.Darken3);
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellHeader).Text("المحامي");
                    header.Cell().Element(CellHeader).AlignCenter().Text("إجمالي القضايا");
                    header.Cell().Element(CellHeader).AlignCenter().Text("المفتوحة");
                    header.Cell().Element(CellHeader).AlignCenter().Text("المغلقة");
                });

                foreach (var row in rows)
                {
                    table.Cell().Element(CellBody).Text(row.LawyerName);
                    table.Cell().Element(CellBody).AlignCenter().Text(row.CasesCount.ToString());
                    table.Cell().Element(CellBody).AlignCenter().Text(row.OpenCases.ToString());
                    table.Cell().Element(CellBody).AlignCenter().Text(row.ClosedCases.ToString());
                }
            });
        });
    }

    private static void RenderPdfCases(IContainer container, string title, IReadOnlyList<ReportCaseRowVM> cases)
    {
        container.Column(col =>
        {
            col.Spacing(6);
            col.Item().Text($"{title} ({cases.Count})").Bold().FontSize(13).FontColor(Colors.Brown.Darken3);
            foreach (var item in cases)
            {
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Column(card =>
                {
                    card.Spacing(3);
                    card.Item().Text($"{item.CaseNumber} / {item.CaseYear}").Bold().FontSize(11);
                    card.Item().Text($"النوع: {item.CaseType}");
                    card.Item().Text($"المحكمة: {item.Court} | الحالة: {item.Status}");
                    card.Item().Text($"المحامي: {item.Lawyers}");
                    card.Item().Text($"تاريخ البداية: {item.StartDate:yyyy/MM/dd} | الأتعاب: {item.FeesAmount:N2} | المسدد: {item.TotalPaid:N2} | المتبقي: {item.Remaining:N2}");
                });
            }
        });
    }

    private sealed class CaseDetailPdf : IDocument
    {
        private readonly CaseReportVM _vm;
        public CaseDetailPdf(CaseReportVM vm) => _vm = vm;

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(18);
                page.ContentFromRightToLeft();
                page.DefaultTextStyle(x => x.FontFamily("Cairo").FontSize(10));

                page.Header().ShowOnce().Element(c => RenderCaseReportHeader(c, _vm));
                page.Content().Column(col =>
                {
                    col.Spacing(8);
                    col.Item().Element(c => RenderCaseQuickSummary(c, _vm));
                    col.Item().Element(c => RenderPdfParagraphBlock(c, "وصف القضية", string.IsNullOrWhiteSpace(_vm.Description) ? "لا يوجد وصف مسجل للقضية." : _vm.Description!));
                    if (_vm.Lawyers.Any())
                    {
                        col.Item().Element(c => RenderPdfLawyerSection(c, _vm.Lawyers.Take(4).ToList()));
                    }
                    if (_vm.Hearings.Any())
                    {
                        col.Item().Element(c => RenderPdfHearingSection(c, _vm.Hearings.Take(3).ToList()));
                    }
                    var activity = _vm.Activity.Take(6).ToList();
                    if (activity.Any())
                    {
                        col.Item().Element(c => RenderPdfActivitySection(c, activity));
                    }
                });

                page.Footer().AlignCenter().Text($"تم التوليد في {DateTime.Now:yyyy/MM/dd HH:mm}");
            });
        }
    }

    private sealed class ConsultationDetailPdf : IDocument
    {
        private readonly ConsultationReportVM _vm;
        public ConsultationDetailPdf(ConsultationReportVM vm) => _vm = vm;

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(18);
                page.ContentFromRightToLeft();
                page.DefaultTextStyle(x => x.FontFamily("Cairo").FontSize(10));

                page.Header().ShowOnce().Element(c => RenderConsultationReportHeader(c, _vm));
                page.Content().Column(col =>
                {
                    col.Spacing(8);
                    col.Item().Element(c => RenderConsultationQuickSummary(c, _vm));
                    col.Item().Element(c => RenderPdfParagraphBlock(c, "الموضوع", string.IsNullOrWhiteSpace(_vm.Subject) ? "لا يوجد موضوع مسجل." : _vm.Subject!));
                    if (!string.IsNullOrWhiteSpace(_vm.LegalOpinion))
                    {
                        col.Item().Element(c => RenderPdfParagraphBlock(c, "الرأي القانوني", _vm.LegalOpinion!));
                    }
                    if (!string.IsNullOrWhiteSpace(_vm.Notes))
                    {
                        col.Item().Element(c => RenderPdfParagraphBlock(c, "ملاحظات", _vm.Notes!));
                    }
                    var activity = _vm.Activity.Take(4).ToList();
                    if (activity.Any())
                    {
                        col.Item().Element(c => RenderConsultationActivitySection(c, activity));
                    }
                });

                page.Footer().AlignCenter().Text($"تم التوليد في {DateTime.Now:yyyy/MM/dd HH:mm}");
            });
        }
    }

    private static void RenderCaseReportHeader(IContainer container, CaseReportVM vm)
    {
        container.Column(col =>
        {
            col.Spacing(8);
            col.Item().Border(1).BorderColor(Colors.Brown.Darken2).Background(Colors.Brown.Lighten5).Padding(14).Column(box =>
            {
                box.Spacing(6);
                box.Item().AlignRight().Text("تقرير القضية").Bold().FontSize(20).FontColor(Colors.Brown.Darken4);
                box.Item().AlignRight().Text(vm.Title).FontSize(12).FontColor(Colors.Grey.Darken1);
                box.Item().Element(c => RenderFactList(c, null, new[]
                {
                    ("رقم القضية", vm.CaseNumber),
                    ("الحالة الحالية", vm.CaseStatus),
                    ("نوع القضية", vm.CaseType),
                    ("العميل", vm.ClientName),
                    ("تاريخ البداية", vm.StartDate.ToString("yyyy/MM/dd")),
                    ("المحكمة", vm.Court ?? "-")
                }));
            });
        });
    }

    private static void RenderConsultationReportHeader(IContainer container, ConsultationReportVM vm)
    {
        container.Column(col =>
        {
            col.Spacing(8);
            col.Item().Border(1).BorderColor(Colors.Brown.Darken2).Background(Colors.Brown.Lighten5).Padding(14).Column(box =>
            {
                box.Spacing(6);
                box.Item().AlignRight().Text("تقرير الاستشارة").Bold().FontSize(20).FontColor(Colors.Brown.Darken4);
                box.Item().AlignRight().Text(vm.Title).FontSize(12).FontColor(Colors.Grey.Darken1);
                box.Item().Element(c => RenderFactList(c, null, new[]
                {
                    ("رقم الاستشارة", vm.ConsultationNumber),
                    ("الحالة", vm.ConsultationStatus),
                    ("العميل", vm.ClientName),
                    ("المستشار", vm.AssignedLawyer),
                    ("النوع", vm.ConsultationType)
                }));
            });
        });
    }

    private static void RenderPdfKeyValueSection(IContainer container, string title, IReadOnlyList<(string Label, string Value)> items)
    {
        RenderFactList(container, title, items);
    }

    private static void RenderCaseQuickSummary(IContainer container, CaseReportVM vm)
    {
        container.Column(col =>
        {
            col.Spacing(4);
            col.Item().Element(c => RenderSectionTitle(c, "ملخص سريع للقضية"));
            col.Item().Row(row =>
            {
                row.RelativeItem().Element(c => RenderFactList(c, null, new[]
                {
                    ("رقم القضية", vm.CaseNumber),
                    ("الحالة الحالية", vm.CaseStatus),
                    ("نوع القضية", vm.CaseType),
                    ("العميل", vm.ClientName)
                }));
                row.ConstantItem(12);
                row.RelativeItem().Element(c => RenderFactList(c, null, new[]
                {
                    ("المحكمة", vm.Court ?? "-"),
                    ("الدائرة", vm.Circuit ?? "-"),
                    ("تاريخ البداية", vm.StartDate.ToString("yyyy/MM/dd")),
                    ("الرسوم / المدفوع", $"{vm.FeesAmount:N2} / {vm.TotalPaid:N2}")
                }));
            });
        });
    }

    private static void RenderConsultationQuickSummary(IContainer container, ConsultationReportVM vm)
    {
        container.Column(col =>
        {
            col.Spacing(4);
            col.Item().Element(c => RenderSectionTitle(c, "ملخص سريع للاستشارة"));
            col.Item().Row(row =>
            {
                row.RelativeItem().Element(c => RenderFactList(c, null, new[]
                {
                    ("رقم الاستشارة", vm.ConsultationNumber),
                    ("الحالة", vm.ConsultationStatus),
                    ("النوع", vm.ConsultationType),
                    ("العميل", vm.ClientName)
                }));
                row.ConstantItem(12);
                row.RelativeItem().Element(c => RenderFactList(c, null, new[]
                {
                    ("المستشار", vm.AssignedLawyer),
                    ("الفرع", vm.Branch ?? "-"),
                    ("القسم", vm.Department ?? "-"),
                    ("الأتعاب", vm.ConsultationFees.ToString("N2"))
                }));
            });
        });
    }

    private static void RenderPdfParagraphBlock(IContainer container, string title, string text)
    {
        container.Column(col =>
        {
            col.Spacing(4);
            col.Item().AlignRight().Text(title).Bold().FontSize(13).FontColor(Colors.Brown.Darken3);
            col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.White).Padding(12).AlignRight().Text(text);
        });
    }

    private static void RenderPdfLawyerSection(IContainer container, IReadOnlyList<CaseReportLawyerRowVM> rows)
    {
        container.Column(col =>
        {
            col.Spacing(4);
            col.Item().Element(c => RenderSectionTitle(c, $"المحامون المرتبطون بالقضية ({rows.Count})"));
            if (!rows.Any())
            {
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).AlignRight().Text("لا يوجد محامون مرتبطون بهذه القضية.");
                return;
            }

            foreach (var row in rows)
            {
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.White).Padding(12).Column(card =>
                {
                    card.Spacing(4);
                    card.Item().AlignRight().Text(row.LawyerName).Bold().FontSize(12);
                    card.Item().AlignRight().Text($"الدور: {row.Role}").FontColor(Colors.Grey.Darken2);
                });
            }
        });
    }

    private static void RenderPdfHearingSection(IContainer container, IReadOnlyList<CaseReportHearingRowVM> rows)
    {
        container.Column(col =>
        {
            col.Spacing(4);
            col.Item().Element(c => RenderSectionTitle(c, $"الجلسات ({rows.Count})"));
            if (!rows.Any())
            {
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).AlignRight().Text("لا توجد جلسات مسجلة.");
                return;
            }

            foreach (var row in rows)
            {
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.White).Padding(12).Column(card =>
                {
                    card.Spacing(3);
                    card.Item().AlignRight().Text($"{row.HearingDate:yyyy/MM/dd}").Bold().FontSize(12);
                    card.Item().AlignRight().Text($"الحالة: {row.HearingStatus}");
                    card.Item().AlignRight().Text($"القرار: {ShortenText(row.CourtDecision, 180)}");
                    card.Item().AlignRight().Text($"الجلسة القادمة: {row.NextHearingDate?.ToString("yyyy/MM/dd") ?? "-"}");
                    card.Item().AlignRight().Text($"المطلوب: {ShortenText(row.NextRequirements, 180)}");
                    card.Item().AlignRight().Text($"ملاحظات: {ShortenText(row.Notes, 180)}");
                });
            }
        });
    }

    private static void RenderPdfPaymentSection(IContainer container, IReadOnlyList<CaseReportPaymentRowVM> rows)
    {
        container.Column(col =>
        {
            col.Spacing(4);
            col.Item().Element(c => RenderSectionTitle(c, $"المدفوعات ({rows.Count})"));
            if (!rows.Any())
            {
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).AlignRight().Text("لا توجد مدفوعات مسجلة.");
                return;
            }

            foreach (var row in rows)
            {
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.White).Padding(12).Column(card =>
                {
                    card.Spacing(3);
                    card.Item().AlignRight().Text($"{row.PaymentDate:yyyy/MM/dd}").Bold().FontSize(12);
                    card.Item().AlignRight().Text($"المبلغ: {row.Amount:N2}");
                    card.Item().AlignRight().Text($"الحالة: {row.Status}");
                    card.Item().AlignRight().Text($"الطريقة: {row.Method ?? "-"}");
                    card.Item().AlignRight().Text($"المرجع: {row.ReferenceNumber ?? "-"}");
                    card.Item().AlignRight().Text($"ملاحظات: {ShortenText(row.Notes, 140)}");
                });
            }
        });
    }

    private static void RenderPdfDocumentSection(IContainer container, IReadOnlyList<CaseReportDocumentRowVM> rows)
    {
        container.Column(col =>
        {
            col.Spacing(4);
            col.Item().Element(c => RenderSectionTitle(c, $"المستندات ({rows.Count})"));
            if (!rows.Any())
            {
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).AlignRight().Text("لا توجد مستندات مرفوعة.");
                return;
            }

            foreach (var row in rows)
            {
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.White).Padding(12).Column(card =>
                {
                    card.Spacing(3);
                    card.Item().AlignRight().Text($"{row.UploadedAt:yyyy/MM/dd}").Bold().FontSize(12);
                    card.Item().AlignRight().Text($"النوع: {row.DocumentType}");
                    card.Item().AlignRight().Text($"الملف: {ShortenText(row.FileName, 60)}");
                    card.Item().AlignRight().Text($"ملاحظات: {ShortenText(row.Notes, 140)}");
                });
            }
        });
    }

    private static void RenderPdfActivitySection(IContainer container, IReadOnlyList<CaseReportActivityVM> rows)
    {
        container.Column(col =>
        {
            col.Spacing(4);
            col.Item().Element(c => RenderSectionTitle(c, $"الخط الزمني ({rows.Count})"));
            if (!rows.Any())
            {
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).AlignRight().Text("لا توجد أحداث مسجلة.");
                return;
            }

            foreach (var row in rows)
            {
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.White).Padding(12).Column(card =>
                {
                    card.Spacing(3);
                    card.Item().AlignRight().Text($"{row.Date:yyyy/MM/dd HH:mm}").Bold().FontSize(12);
                    card.Item().AlignRight().Text($"النوع: {row.Category}");
                    card.Item().AlignRight().Text($"العنوان: {row.Title}");
                    if (!string.IsNullOrWhiteSpace(row.Description))
                    {
                        card.Item().AlignRight().Text($"التفاصيل: {ShortenText(row.Description, 180)}").FontColor(Colors.Grey.Darken1);
                    }
                });
            }
        });
    }

    private static void RenderConsultationActivitySection(IContainer container, IReadOnlyList<ConsultationReportActivityVM> rows)
    {
        container.Column(col =>
        {
            col.Spacing(4);
            col.Item().Element(c => RenderSectionTitle(c, $"الخط الزمني ({rows.Count})"));
            if (!rows.Any())
            {
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).AlignRight().Text("لا توجد أحداث مسجلة.");
                return;
            }

            foreach (var row in rows)
            {
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.White).Padding(12).Column(card =>
                {
                    card.Spacing(3);
                    card.Item().AlignRight().Text($"{row.Date:yyyy/MM/dd HH:mm}").Bold().FontSize(12);
                    card.Item().AlignRight().Text($"النوع: {row.Category}");
                    card.Item().AlignRight().Text($"العنوان: {row.Title}");
                    if (!string.IsNullOrWhiteSpace(row.Description))
                    {
                        card.Item().AlignRight().Text($"التفاصيل: {ShortenText(row.Description, 180)}").FontColor(Colors.Grey.Darken1);
                    }
                });
            }
        });
    }

    private static void RenderFactList(IContainer container, string? title, IReadOnlyList<(string Label, string Value)> items)
    {
        container.Column(col =>
        {
            col.Spacing(4);
            if (!string.IsNullOrWhiteSpace(title))
            {
                col.Item().AlignRight().Text(title).Bold().FontSize(13).FontColor(Colors.Brown.Darken3);
            }

            for (var i = 0; i < items.Count; i += 2)
            {
                var first = items[i];
                var hasSecond = i + 1 < items.Count;
                var second = hasSecond ? items[i + 1] : default;

                col.Item().Row(row =>
                {
                    row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.White).Padding(8).Column(card =>
                    {
                        card.Spacing(2);
                        card.Item().AlignRight().Text(first.Label).Bold().FontColor(Colors.Grey.Darken3);
                        card.Item().AlignRight().Text(string.IsNullOrWhiteSpace(first.Value) ? "-" : first.Value).FontSize(11);
                    });

                    if (hasSecond)
                    {
                        row.ConstantItem(8);
                        row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.White).Padding(8).Column(card =>
                        {
                            card.Spacing(2);
                            card.Item().AlignRight().Text(second.Label).Bold().FontColor(Colors.Grey.Darken3);
                            card.Item().AlignRight().Text(string.IsNullOrWhiteSpace(second.Value) ? "-" : second.Value).FontSize(11);
                        });
                    }
                });
            }
        });
    }

    private static void RenderSectionTitle(IContainer container, string title)
    {
        container.Border(1)
            .BorderColor(Colors.Brown.Darken2)
            .Background(Colors.Brown.Lighten5)
            .PaddingVertical(8)
            .PaddingHorizontal(10)
            .AlignRight()
            .Text(title)
            .Bold()
            .FontSize(13)
            .FontColor(Colors.Brown.Darken4);
    }

    private static IContainer CellHeader(IContainer container) =>
        container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Background(Colors.Grey.Lighten4)
            .Padding(5)
            .DefaultTextStyle(x => x.Bold());

    private static IContainer CellBody(IContainer container) =>
        container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5);
}
