using LegalOffice.Application.ViewModels;
using LegalOffice.Domain.Entities;
using LegalOffice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class PaymentsController : Controller
{
    private readonly AppDbContext _db;

    public PaymentsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Create(int caseId)
    {
        if (await IsCaseClosedAsync(caseId))
        {
            TempData["ToastError"] = "لا يمكن إضافة دفعات لقضية مغلقة.";
            return RedirectToAction("Details", "Cases", new { id = caseId });
        }

        var vm = new PaymentCreateEditVM { CaseId = caseId, PaymentDate = DateTime.Today };
        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PaymentCreateEditVM vm)
    {
        if (await IsCaseClosedAsync(vm.CaseId))
        {
            TempData["ToastError"] = "لا يمكن إضافة دفعات لقضية مغلقة.";
            return RedirectToAction("Details", "Cases", new { id = vm.CaseId });
        }

        if (!ModelState.IsValid)
        {
            await Fill(vm);
            return View(vm);
        }

        _db.Payments.Add(new Payment
        {
            CaseId = vm.CaseId,
            Amount = vm.Amount,
            PaymentDate = vm.PaymentDate,
            PaymentStatusId = vm.PaymentStatusId,
            PaymentMethodId = vm.PaymentMethodId,
            ReferenceNumber = vm.ReferenceNumber,
            Notes = vm.Notes,
            ReceivedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        });

        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تمت إضافة الدفعة بنجاح.";
        return RedirectToAction("Details", "Cases", new { id = vm.CaseId });
    }

    private async Task Fill(PaymentCreateEditVM vm)
    {
        vm.PaymentStatuses = await SelectLookups("PaymentStatus");
        vm.PaymentMethods = await SelectLookups("PaymentMethod");
    }

    private async Task<List<SelectListItem>> SelectLookups(string type)
    {
        return await _db.Lookups
            .Where(x => x.Type == type && x.IsActive)
            .OrderBy(x => x.NameAr)
            .Select(x => new SelectListItem(x.NameAr, x.Id.ToString()))
            .ToListAsync();
    }

    private async Task<bool> IsCaseClosedAsync(int caseId)
    {
        var closedId = await _db.Lookups
            .Where(x => x.Type == "CaseStatus" && x.NameEn == "Closed")
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        return closedId > 0 && await _db.Cases.AnyAsync(x => x.Id == caseId && x.CaseStatusId == closedId);
    }
}
