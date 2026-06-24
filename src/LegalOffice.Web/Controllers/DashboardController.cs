using LegalOffice.Application.ViewModels;
using LegalOffice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using LegalOffice.Web.Services;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;

    public DashboardController(AppDbContext db, IPermissionService permissions)
    {
        _db = db;
        _permissions = permissions;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var weekEnd = today.AddDays(7);
        var currentLawyerId = await GetCurrentLawyerIdAsync();

        var casesQuery = _db.Cases.AsQueryable();
        var hearingsQuery = _db.CaseHearings.AsQueryable();
        var paymentsQuery = _db.Payments.AsQueryable();

        var canViewAllData = await _permissions.HasPermissionAsync(User, "Dashboard.ViewAll");

        if (!canViewAllData && currentLawyerId.HasValue)
        {
            casesQuery = casesQuery.Where(x => x.CaseLawyers.Any(cl => cl.LawyerId == currentLawyerId.Value));
            hearingsQuery = hearingsQuery.Where(x => x.Case.CaseLawyers.Any(cl => cl.LawyerId == currentLawyerId.Value));
            paymentsQuery = paymentsQuery.Where(x => x.Case.CaseLawyers.Any(cl => cl.LawyerId == currentLawyerId.Value));
        }
        else if (!canViewAllData)
        {
            casesQuery = casesQuery.Where(x => false);
            hearingsQuery = hearingsQuery.Where(x => false);
            paymentsQuery = paymentsQuery.Where(x => false);
        }

        var totalCases = await casesQuery.CountAsync();
        var totalFees = await casesQuery.SumAsync(x => (decimal?)x.FeesAmount) ?? 0;
        var totalPaid = await paymentsQuery
            .Where(x => x.PaymentStatus.NameAr == "مستلم")
            .SumAsync(x => (decimal?)x.Amount) ?? 0;

        var vm = new DashboardVM
        {
            TotalCases = totalCases,
            OpenCases = await casesQuery.CountAsync(x => x.ClosedDate == null),
            ClosedCases = await casesQuery.CountAsync(x => x.ClosedDate != null),
            TodayHearings = await hearingsQuery.CountAsync(x => x.HearingDate.Date == today),
            WeekHearings = await hearingsQuery.CountAsync(x => x.HearingDate.Date >= today && x.HearingDate.Date <= weekEnd),
            MonthPayments = await paymentsQuery.Where(x => x.PaymentDate.Month == today.Month && x.PaymentDate.Year == today.Year).SumAsync(x => (decimal?)x.Amount) ?? 0,
            TotalRemaining = Math.Max(totalFees - totalPaid, 0),
            MyCases = currentLawyerId.HasValue ? await casesQuery.CountAsync() : totalCases
        };

        return View(vm);
    }

    private async Task<int?> GetCurrentLawyerIdAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        return await _db.Lawyers
            .Where(x => x.UserId == userId)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync();
    }
}
