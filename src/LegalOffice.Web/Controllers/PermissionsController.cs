using LegalOffice.Application.ViewModels;
using LegalOffice.Domain.Entities;
using LegalOffice.Infrastructure.Persistence;
using LegalOffice.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class PermissionsController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;

    public PermissionsController(AppDbContext db, IPermissionService permissions)
    {
        _db = db;
        _permissions = permissions;
    }

    public async Task<IActionResult> Index()
    {
        var roles = await _db.Roles
            .OrderBy(x => x.Name)
            .Select(x => x.Name!)
            .ToListAsync();

        return View(roles);
    }

    public async Task<IActionResult> EditRole(string roleName)
    {
        var permissions = await _db.SystemPermissions.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.MenuGroup)
            .ThenBy(x => x.SortOrder)
            .ToListAsync();

        return View(new RolePermissionEditVM
        {
            RoleName = roleName,
            RoleDisplayName = roleName,
            Permissions = permissions,
            SelectedPermissionIds = (await _permissions.GetRolePermissionIdsAsync(roleName)).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRole(RolePermissionEditVM vm)
    {
        var roleExists = await _db.Roles.AnyAsync(x => x.Name == vm.RoleName);
        if (!roleExists)
        {
            return NotFound();
        }

        var permissionIds = vm.SelectedPermissionIds?.Where(x => x > 0).Distinct().ToList() ?? new List<int>();
        var existing = await _db.RolePermissions.Where(x => x.RoleName == vm.RoleName).ToListAsync();
        _db.RolePermissions.RemoveRange(existing);

        foreach (var permissionId in permissionIds)
        {
            _db.RolePermissions.Add(new RolePermission
            {
                RoleName = vm.RoleName,
                SystemPermissionId = permissionId
            });
        }

        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تم حفظ الصلاحيات بنجاح.";
        return RedirectToAction(nameof(Index));
    }
}
