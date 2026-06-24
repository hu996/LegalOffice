using LegalOffice.Application.ViewModels;
using LegalOffice.Domain.Entities;
using LegalOffice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class LookupsController : Controller
{
    private readonly AppDbContext _db;

    public LookupsController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 50);

        var query = _db.LookupTypes.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Code.Contains(search) ||
                x.NameAr.Contains(search) ||
                (x.NameEn ?? "").Contains(search));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.NameAr)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Search = search;
        return View(new LookupTypeIndexVM
        {
            Types = new PagedResult<LookupType>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            },
            Search = search
        });
    }

    public async Task<IActionResult> Items(int lookupTypeId, string? search, int page = 1, int pageSize = 10)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 50);

        var lookupType = await _db.LookupTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == lookupTypeId);
        if (lookupType == null)
        {
            return NotFound();
        }

        var query = _db.Lookups.AsNoTracking().Where(x => x.LookupTypeId == lookupTypeId);
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.NameAr.Contains(search) || (x.NameEn ?? "").Contains(search));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.IsActive)
            .ThenBy(x => x.NameAr)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Search = search;
        return View(new LookupItemIndexVM
        {
            LookupType = lookupType,
            Items = new PagedResult<Lookup>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            }
        });
    }

    public IActionResult CreateType() => View(new LookupType { IsActive = true, SortOrder = 0 });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateType(LookupType model)
    {
        NormalizeLookupType(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (await _db.LookupTypes.AnyAsync(x => x.Code == model.Code))
        {
            ModelState.AddModelError(nameof(model.Code), "النوع ده موجود بالفعل.");
            return View(model);
        }

        _db.LookupTypes.Add(model);
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تمت إضافة نوع الـ lookup بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> EditType(int id)
    {
        var item = await _db.LookupTypes.FirstOrDefaultAsync(x => x.Id == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditType(LookupType model)
    {
        NormalizeLookupType(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var item = await _db.LookupTypes.FirstOrDefaultAsync(x => x.Id == model.Id);
        if (item == null)
        {
            return NotFound();
        }

        if (!string.Equals(item.Code, model.Code, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.Code), "لا يمكن تعديل كود النوع بعد إنشائه.");
            return View(model);
        }

        item.NameAr = model.NameAr;
        item.NameEn = model.NameEn;
        item.IsActive = model.IsActive;
        item.SortOrder = model.SortOrder;

        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تم تحديث نوع الـ lookup بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleTypeStatus(int id)
    {
        var item = await _db.LookupTypes.FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return NotFound();
        }

        item.IsActive = !item.IsActive;
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = item.IsActive ? "تم تفعيل النوع." : "تم تعطيل النوع.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteType(int id)
    {
        var item = await _db.LookupTypes.Include(x => x.Lookups).FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return NotFound();
        }

        if (item.Lookups.Any())
        {
            TempData["ToastError"] = "لا يمكن حذف النوع لأنه يحتوي على بيانات مرتبطة.";
            return RedirectToAction(nameof(Index));
        }

        _db.LookupTypes.Remove(item);
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تم حذف النوع بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Create(int lookupTypeId)
    {
        var lookupType = await _db.LookupTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == lookupTypeId);
        if (lookupType == null)
        {
            return NotFound();
        }

        ViewBag.LookupTypeName = lookupType.NameAr;
        return View(new Lookup
        {
            LookupTypeId = lookupType.Id,
            Type = lookupType.Code,
            IsActive = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Lookup model)
    {
        var lookupType = await LoadLookupTypeAsync(model.LookupTypeId);
        if (lookupType == null)
        {
            return NotFound();
        }

        model.Type = lookupType.Code;
        if (!ModelState.IsValid)
        {
            ViewBag.LookupTypeName = lookupType?.NameAr ?? string.Empty;
            return View(model);
        }

        if (await _db.Lookups.AnyAsync(x => x.LookupTypeId == model.LookupTypeId && x.NameAr == model.NameAr))
        {
            ModelState.AddModelError(nameof(model.NameAr), "القيمة دي موجودة بالفعل داخل نفس النوع.");
            ViewBag.LookupTypeName = lookupType.NameAr;
            return View(model);
        }

        _db.Lookups.Add(model);
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تمت إضافة القيمة بنجاح.";
        return RedirectToAction(nameof(Items), new { lookupTypeId = model.LookupTypeId });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.Lookups.Include(x => x.LookupType).FirstOrDefaultAsync(x => x.Id == id);
        if (item != null)
        {
            ViewBag.LookupTypeName = item.LookupType?.NameAr ?? string.Empty;
        }
        return item == null ? NotFound() : View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Lookup model)
    {
        var existing = await _db.Lookups.Include(x => x.LookupType).FirstOrDefaultAsync(x => x.Id == model.Id);
        if (existing == null)
        {
            return NotFound();
        }

        model.LookupTypeId = existing.LookupTypeId;
        model.Type = existing.Type;

        if (!ModelState.IsValid)
        {
            ViewBag.LookupTypeName = existing.LookupType?.NameAr ?? string.Empty;
            return View(model);
        }

        var duplicateExists = await _db.Lookups.AnyAsync(x =>
            x.LookupTypeId == existing.LookupTypeId &&
            x.NameAr == model.NameAr &&
            x.Id != model.Id);

        if (duplicateExists)
        {
            ModelState.AddModelError(nameof(model.NameAr), "القيمة دي موجودة بالفعل داخل نفس النوع.");
            ViewBag.LookupTypeName = existing.LookupType?.NameAr ?? string.Empty;
            return View(model);
        }

        existing.NameAr = model.NameAr;
        existing.NameEn = model.NameEn;
        existing.IsActive = model.IsActive;

        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تم تحديث القيمة بنجاح.";
        return RedirectToAction(nameof(Items), new { lookupTypeId = existing.LookupTypeId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var item = await _db.Lookups.FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return NotFound();
        }

        item.IsActive = !item.IsActive;
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = item.IsActive ? "تم تفعيل القيمة." : "تم تعطيل القيمة.";
        return RedirectToAction(nameof(Items), new { lookupTypeId = item.LookupTypeId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Lookups.FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return NotFound();
        }

        if (await IsLookupInUseAsync(item.Id))
        {
            TempData["ToastError"] = "لا يمكن حذف القيمة لأنها مستخدمة داخل النظام.";
            return RedirectToAction(nameof(Items), new { lookupTypeId = item.LookupTypeId });
        }

        _db.Lookups.Remove(item);
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تم حذف القيمة بنجاح.";
        return RedirectToAction(nameof(Items), new { lookupTypeId = item.LookupTypeId });
    }

    private static void NormalizeLookupType(LookupType model)
    {
        model.Code = model.Code?.Trim() ?? string.Empty;
        model.NameAr = model.NameAr?.Trim() ?? string.Empty;
        model.NameEn = model.NameEn?.Trim();
    }

    private async Task<LookupType?> LoadLookupTypeAsync(int lookupTypeId)
    {
        return await _db.LookupTypes.FirstOrDefaultAsync(x => x.Id == lookupTypeId);
    }

    private async Task<bool> IsLookupInUseAsync(int lookupId)
    {
        return await _db.Cases.AnyAsync(x => x.CaseTypeId == lookupId || x.CaseStatusId == lookupId || x.CourtId == lookupId || x.PriorityId == lookupId)
            || await _db.Cases.AnyAsync(x => x.DepartmentId == lookupId || x.WorkflowStageLookupId == lookupId)
            || await _db.CaseLawyers.AnyAsync(x => x.AccessLevelId == lookupId)
            || await _db.CaseHearings.AnyAsync(x => x.HearingStatusId == lookupId)
            || await _db.CaseDocuments.AnyAsync(x => x.DocumentTypeId == lookupId)
            || await _db.Payments.AnyAsync(x => x.PaymentStatusId == lookupId || x.PaymentMethodId == lookupId)
            || await _db.Expenses.AnyAsync(x => x.ExpenseTypeId == lookupId || x.StatusLookupId == lookupId)
            || await _db.LawyerSpecialties.AnyAsync(x => x.CaseTypeId == lookupId)
            || await _db.Lawyers.AnyAsync(x => x.DepartmentId == lookupId)
            || await _db.Users.AnyAsync(x => x.DepartmentId == lookupId || x.UserTypeId == lookupId);
    }
}
