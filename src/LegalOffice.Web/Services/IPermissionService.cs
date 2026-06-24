using LegalOffice.Application.ViewModels;
using System.Security.Claims;

namespace LegalOffice.Web.Services;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permissionCode);
    Task<IReadOnlyList<SidebarMenuGroupVM>> GetSidebarGroupsAsync(ClaimsPrincipal user);
    Task<IReadOnlyList<int>> GetRolePermissionIdsAsync(string roleName);
}
