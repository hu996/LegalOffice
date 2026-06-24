using LegalOffice.Application.ViewModels;
using LegalOffice.Domain.Entities;
using LegalOffice.Infrastructure.Persistence;
using LegalOffice.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class TreasuryController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;
    private readonly ICaseTypeOptionsService _caseTypeOptions;

    public TreasuryController(AppDbContext db, IPermissionService permissions, ICaseTypeOptionsService caseTypeOptions)
    {
        _db = db;
        _permissions = permissions;
        _caseTypeOptions = caseTypeOptions;
    }

    public async Task<IActionResult> Index()
    {
        if (!await _permissions.HasPermissionAsync(User, "Treasury.View"))
        {
            return Forbid();
        }

        var treasury = await GetOrCreateTreasuryAsync();
        var transactions = await _db.TreasuryTransactions
            .AsNoTracking()
            .Include(x => x.TransactionTypeLookup)
            .Include(x => x.Case)
            .Where(x => x.TreasuryId == treasury.Id)
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync();

        var vm = new TreasuryDashboardVM
        {
            TreasuryId = treasury.Id,
            TreasuryName = treasury.NameAr,
            CurrentBalance = treasury.CurrentBalance,
            TotalIncome = transactions.Where(x => IsIncome(x.TransactionTypeLookup?.NameAr)).Sum(x => x.Amount),
            TotalExpense = transactions.Where(x => !IsIncome(x.TransactionTypeLookup?.NameAr)).Sum(x => x.Amount),
            Transactions = transactions.Select(x => new TreasuryTransactionListItemVM
            {
                Id = x.Id,
                TransactionTypeName = x.TransactionTypeLookup?.NameAr ?? string.Empty,
                Amount = x.Amount,
                Notes = x.Notes,
                TransactionDate = x.TransactionDate,
                CaseNumber = x.Case?.CaseNumber
            }).ToList()
        };

        return View(vm);
    }

    public async Task<IActionResult> Create()
    {
        if (!await _permissions.HasPermissionAsync(User, "Treasury.Create"))
        {
            return Forbid();
        }

        var treasury = await GetOrCreateTreasuryAsync();
        var vm = new TreasuryTransactionVM
        {
            TreasuryId = treasury.Id,
            TransactionDate = DateTime.Now
        };
        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TreasuryTransactionVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "Treasury.Create"))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            await Fill(vm);
            return View(vm);
        }

        if (!await ValidateCaseTypeMatchAsync(vm.CaseId, vm.CaseTypeId))
        {
            await Fill(vm);
            return View(vm);
        }

        var treasury = await _db.OfficeTreasuries.FirstOrDefaultAsync(x => x.Id == vm.TreasuryId);
        if (treasury == null)
        {
            return NotFound();
        }

        var typeName = await _db.Lookups.Where(x => x.Id == vm.TransactionTypeLookupId).Select(x => x.NameAr).FirstOrDefaultAsync();
        var isIncome = IsIncome(typeName);

        _db.TreasuryTransactions.Add(new TreasuryTransaction
        {
            TreasuryId = treasury.Id,
            TransactionTypeLookupId = vm.TransactionTypeLookupId,
            Amount = vm.Amount,
            Notes = vm.Notes,
            TransactionDate = vm.TransactionDate,
            CaseId = vm.CaseId
        });

        treasury.CurrentBalance = isIncome ? treasury.CurrentBalance + vm.Amount : treasury.CurrentBalance - vm.Amount;
        await _db.SaveChangesAsync();

        TempData["ToastSuccess"] = "تم تسجيل حركة الخزنة بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<OfficeTreasury> GetOrCreateTreasuryAsync()
    {
        var treasury = await _db.OfficeTreasuries.FirstOrDefaultAsync();
        if (treasury != null)
        {
            return treasury;
        }

        treasury = new OfficeTreasury { NameAr = "خزنة المكتب", CurrentBalance = 0, CreatedAt = DateTime.Now };
        _db.OfficeTreasuries.Add(treasury);
        await _db.SaveChangesAsync();
        return treasury;
    }

    private async Task Fill(TreasuryTransactionVM vm)
    {
        vm.CaseTypes = await _caseTypeOptions.GetVisibleCaseTypesAsync(User);
        vm.Treasuries = await _db.OfficeTreasuries.AsNoTracking()
            .OrderBy(x => x.NameAr)
            .Select(x => new SelectListItem(x.NameAr, x.Id.ToString()))
            .ToListAsync();
        vm.TransactionTypes = await _db.Lookups
            .Where(x => x.Type == "TransactionType" && x.IsActive)
            .OrderBy(x => x.NameAr)
            .Select(x => new SelectListItem(x.NameAr, x.Id.ToString()))
            .ToListAsync();
        vm.Cases = await BuildCasesAsync(vm.CaseTypeId);
    }

    private async Task<List<SelectListItem>> BuildCasesAsync(int? caseTypeId)
    {
        var query = _db.Cases.AsNoTracking().Include(x => x.Client).AsQueryable();
        if (caseTypeId.HasValue)
        {
            query = query.Where(x => x.CaseTypeId == caseTypeId.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new SelectListItem($"{x.CaseNumber} - {x.Client.FullName}", x.Id.ToString()))
            .ToListAsync();
    }

    private async Task<bool> ValidateCaseTypeMatchAsync(int? caseId, int? caseTypeId)
    {
        if (!caseId.HasValue || !caseTypeId.HasValue)
        {
            return true;
        }

        var matches = await _db.Cases.AnyAsync(x => x.Id == caseId.Value && x.CaseTypeId == caseTypeId.Value);
        if (!matches)
        {
            ModelState.AddModelError(nameof(TreasuryTransactionVM.CaseId), "القضية المختارة لا تطابق نوع القضية.");
            TempData["ToastError"] = "القضية المختارة لا تطابق نوع القضية.";
        }

        return matches;
    }

    private static bool IsIncome(string? transactionTypeNameAr)
    {
        if (string.IsNullOrWhiteSpace(transactionTypeNameAr))
        {
            return true;
        }

        return transactionTypeNameAr.Contains("إيراد") ||
               transactionTypeNameAr.Contains("دخل") ||
               transactionTypeNameAr.Contains("استلام");
    }
}
