using LegalOffice.Application.ViewModels;
using LegalOffice.Domain.Entities;
using LegalOffice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class CourtsController : Controller
{
    private const string CourtType = "Court";
    private readonly AppDbContext _db;

    public CourtsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 50);

        var query = _db.Lookups
            .Include(x => x.LookupType)
            .Where(x => x.Type == CourtType);

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
        return View(new PagedResult<Lookup>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }

    public async Task<IActionResult> Create()
    {
        var lookupType = await _db.LookupTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Code == CourtType);
        if (lookupType == null)
        {
            return NotFound();
        }

        return View(new Lookup { Type = CourtType, LookupTypeId = lookupType.Id, IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Lookup model)
    {
        var lookupType = await _db.LookupTypes.FirstOrDefaultAsync(x => x.Code == CourtType);
        if (lookupType == null)
        {
            return NotFound();
        }

        model.Type = CourtType;
        model.LookupTypeId = lookupType.Id;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var exists = await _db.Lookups.AnyAsync(x => x.LookupTypeId == lookupType.Id && x.NameAr == model.NameAr);
        if (exists)
        {
            ModelState.AddModelError(nameof(model.NameAr), "المحكمة دي موجودة بالفعل.");
            return View(model);
        }

        _db.Lookups.Add(model);
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تمت إضافة المحكمة بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.Lookups.Include(x => x.LookupType).FirstOrDefaultAsync(x => x.Id == id && x.Type == CourtType);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Lookup model)
    {
        var lookupType = await _db.LookupTypes.FirstOrDefaultAsync(x => x.Code == CourtType);
        if (lookupType == null)
        {
            return NotFound();
        }

        model.Type = CourtType;
        model.LookupTypeId = lookupType.Id;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var exists = await _db.Lookups.AnyAsync(x => x.LookupTypeId == lookupType.Id && x.NameAr == model.NameAr && x.Id != model.Id);
        if (exists)
        {
            ModelState.AddModelError(nameof(model.NameAr), "المحكمة دي موجودة بالفعل.");
            return View(model);
        }

        _db.Lookups.Update(model);
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تم تحديث المحكمة بنجاح.";
        return RedirectToAction(nameof(Index));
    }
}
