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
public class BranchesController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;

    public BranchesController(AppDbContext db, IPermissionService permissions)
    {
        _db = db;
        _permissions = permissions;
    }

    public async Task<IActionResult> Index(string? search)
    {
        if (!await _permissions.HasPermissionAsync(User, "Branches.View"))
        {
            return Forbid();
        }

        var query = _db.Branches
            .AsNoTracking()
            .Include(x => x.ManagerUser)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.NameAr.Contains(search) || (x.Address ?? "").Contains(search) || (x.Phone ?? "").Contains(search));
        }

        var items = await query
            .OrderByDescending(x => x.IsActive)
            .ThenBy(x => x.NameAr)
            .Select(x => new BranchListItemVM
            {
                Id = x.Id,
                NameAr = x.NameAr,
                Address = x.Address,
                Phone = x.Phone,
                IsActive = x.IsActive,
                ManagerName = x.ManagerUser != null ? x.ManagerUser.FullName : null
            })
            .ToListAsync();

        ViewBag.Search = search;
        return View(items);
    }

    public async Task<IActionResult> Create()
    {
        if (!await _permissions.HasPermissionAsync(User, "Branches.Create"))
        {
            return Forbid();
        }

        var vm = new BranchEditVM();
        await FillManagers(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BranchEditVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "Branches.Create"))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            await FillManagers(vm);
            return View(vm);
        }

        _db.Branches.Add(new Branch
        {
            NameAr = vm.NameAr.Trim(),
            Address = vm.Address,
            Phone = vm.Phone,
            ManagerUserId = vm.ManagerUserId,
            IsActive = vm.IsActive
        });

        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تمت إضافة الفرع بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!await _permissions.HasPermissionAsync(User, "Branches.Edit"))
        {
            return Forbid();
        }

        var entity = await _db.Branches.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        var vm = new BranchEditVM
        {
            Id = entity.Id,
            NameAr = entity.NameAr,
            Address = entity.Address,
            Phone = entity.Phone,
            ManagerUserId = entity.ManagerUserId,
            IsActive = entity.IsActive
        };

        await FillManagers(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BranchEditVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "Branches.Edit"))
        {
            return Forbid();
        }

        if (!vm.Id.HasValue)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await FillManagers(vm);
            return View(vm);
        }

        var entity = await _db.Branches.FirstOrDefaultAsync(x => x.Id == vm.Id.Value);
        if (entity == null)
        {
            return NotFound();
        }

        entity.NameAr = vm.NameAr.Trim();
        entity.Address = vm.Address;
        entity.Phone = vm.Phone;
        entity.ManagerUserId = vm.ManagerUserId;
        entity.IsActive = vm.IsActive;
        await _db.SaveChangesAsync();

        TempData["ToastSuccess"] = "تم تعديل الفرع بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _permissions.HasPermissionAsync(User, "Branches.Delete"))
        {
            return Forbid();
        }

        var entity = await _db.Branches.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        var inUse = await _db.Cases.AnyAsync(x => x.BranchId == id)
            || await _db.Clients.AnyAsync(x => x.BranchId == id)
            || await _db.Lawyers.AnyAsync(x => x.BranchId == id)
            || await _db.Users.AnyAsync(x => x.BranchId == id);

        if (inUse)
        {
            TempData["ToastError"] = "لا يمكن حذف الفرع لأنه مستخدم داخل النظام.";
            return RedirectToAction(nameof(Index));
        }

        _db.Branches.Remove(entity);
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تم حذف الفرع بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    private async Task FillManagers(BranchEditVM vm)
    {
        vm.Managers = await _db.Users
            .OrderBy(x => x.FullName)
            .Select(x => new SelectListItem(x.FullName, x.Id.ToString()))
            .ToListAsync();
    }
}
