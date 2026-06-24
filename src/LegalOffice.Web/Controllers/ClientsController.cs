using LegalOffice.Domain.Entities;
using LegalOffice.Application.ViewModels;
using LegalOffice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class ClientsController : Controller
{
    private readonly AppDbContext _db;

    public ClientsController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 50);

        var query = _db.Clients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.FullName.Contains(search) ||
                (x.Phone ?? "").Contains(search) ||
                (x.NationalId ?? "").Contains(search));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Search = search;
        return View(new PagedResult<Client>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }

    public IActionResult Create() => View(new Client());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Client model)
    {
        await ValidateUniqueNationalIdAsync(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _db.Clients.Add(model);
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تم حفظ العميل بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.Clients.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Client model)
    {
        await ValidateUniqueNationalIdAsync(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _db.Clients.Update(model);
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تم تعديل بيانات العميل بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _db.Clients.Include(x => x.Cases).ThenInclude(x => x.CaseStatus).FirstOrDefaultAsync(x => x.Id == id);
        return item == null ? NotFound() : View(item);
    }

    private async Task ValidateUniqueNationalIdAsync(Client model)
    {
        if (string.IsNullOrWhiteSpace(model.NationalId))
        {
            return;
        }

        var exists = await _db.Clients.AnyAsync(x => x.NationalId == model.NationalId && x.Id != model.Id);
        if (exists)
        {
            ModelState.AddModelError(nameof(model.NationalId), "الرقم القومي موجود بالفعل لعميل آخر.");
            TempData["ToastError"] = "الرقم القومي موجود بالفعل لعميل آخر.";
        }
    }
}
