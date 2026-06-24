using LegalOffice.Application.ViewModels;
using LegalOffice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LegalOffice.Web.Services;

public class PermissionService : IPermissionService
{
    private readonly AppDbContext _db;

    public PermissionService(AppDbContext db) => _db = db;

    public async Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permissionCode)
    {
        if (user.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        var roleNames = user.Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value)
            .Distinct()
            .ToList();

        if (roleNames.Count == 0)
        {
            return false;
        }

        if (roleNames.Contains("Admin"))
        {
            return true;
        }

        return await _db.RolePermissions
            .Include(x => x.SystemPermission)
            .AnyAsync(x =>
                roleNames.Contains(x.RoleName) &&
                x.SystemPermission.Code == permissionCode &&
                x.SystemPermission.IsActive);
    }

    public async Task<IReadOnlyList<SidebarMenuGroupVM>> GetSidebarGroupsAsync(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
        {
            return Array.Empty<SidebarMenuGroupVM>();
        }

        var roleNames = user.Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value)
            .Distinct()
            .ToList();

        if (roleNames.Contains("Admin"))
        {
            var allItems = await _db.SystemPermissions
                .AsNoTracking()
                .Where(x => x.IsActive && x.IsMenuItem)
                .OrderBy(x => x.MenuGroup)
                .ThenBy(x => x.SortOrder)
                .Select(x => new
                {
                    x.MenuGroup,
                    x.NameAr,
                    x.Controller,
                    x.Action,
                    x.Code
                })
                .ToListAsync();

            return allItems
                .GroupBy(x => string.IsNullOrWhiteSpace(x.MenuGroup) ? "عام" : x.MenuGroup!)
                .Select(group => new SidebarMenuGroupVM
                {
                    Title = group.Key,
                    Items = group.Select(x => new SidebarMenuItemVM
                    {
                        Label = x.NameAr,
                        Controller = x.Controller,
                        Action = x.Action,
                        PermissionCode = x.Code
                    }).ToList()
                })
                .ToList();
        }

        var query = _db.SystemPermissions
            .AsNoTracking()
            .Where(x => x.IsActive && x.IsMenuItem);

        query = query.Where(p => _db.RolePermissions.Any(rp => roleNames.Contains(rp.RoleName) && rp.SystemPermissionId == p.Id));

        var items = await query
            .OrderBy(x => x.MenuGroup)
            .ThenBy(x => x.SortOrder)
            .Select(x => new
            {
                x.MenuGroup,
                x.NameAr,
                x.Controller,
                x.Action,
                x.Code
            })
            .ToListAsync();

        return items
            .GroupBy(x => string.IsNullOrWhiteSpace(x.MenuGroup) ? "عام" : x.MenuGroup!)
            .Select(group => new SidebarMenuGroupVM
            {
                Title = group.Key,
                Items = group.Select(x => new SidebarMenuItemVM
                {
                    Label = x.NameAr,
                    Controller = x.Controller,
                    Action = x.Action,
                    PermissionCode = x.Code
                }).ToList()
            })
            .ToList();
    }

    public async Task<IReadOnlyList<int>> GetRolePermissionIdsAsync(string roleName)
    {
        return await _db.RolePermissions
            .Where(x => x.RoleName == roleName)
            .Select(x => x.SystemPermissionId)
            .ToListAsync();
    }
}
