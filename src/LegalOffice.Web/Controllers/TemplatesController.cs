using LegalOffice.Domain.Entities;
using LegalOffice.Infrastructure.Persistence;
using LegalOffice.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class TemplatesController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;

    public TemplatesController(AppDbContext db, IPermissionService permissions)
    {
        _db = db;
        _permissions = permissions;
    }

    public async Task<IActionResult> Index()
    {
        if (!await _permissions.HasPermissionAsync(User, "Templates.View"))
        {
            return Forbid();
        }

        return View(await _db.MessageTemplates.AsNoTracking().OrderBy(x => x.Name).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        if (!await _permissions.HasPermissionAsync(User, "Templates.Create"))
        {
            return Forbid();
        }

        return View(new MessageTemplate { IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MessageTemplate model)
    {
        if (!await _permissions.HasPermissionAsync(User, "Templates.Create"))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _db.MessageTemplates.Add(model);
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تمت إضافة القالب بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!await _permissions.HasPermissionAsync(User, "Templates.Edit"))
        {
            return Forbid();
        }

        var model = await _db.MessageTemplates.FindAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MessageTemplate model)
    {
        if (!await _permissions.HasPermissionAsync(User, "Templates.Edit"))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existing = await _db.MessageTemplates.FirstOrDefaultAsync(x => x.Id == model.Id);
        if (existing == null)
        {
            return NotFound();
        }

        existing.Name = model.Name.Trim();
        existing.Channel = model.Channel.Trim();
        existing.Body = model.Body.Trim();
        existing.IsActive = model.IsActive;
        await _db.SaveChangesAsync();

        TempData["ToastSuccess"] = "تم تحديث القالب بنجاح.";
        return RedirectToAction(nameof(Index));
    }
}
