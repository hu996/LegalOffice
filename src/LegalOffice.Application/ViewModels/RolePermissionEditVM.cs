using LegalOffice.Domain.Entities;

namespace LegalOffice.Application.ViewModels;

public class RolePermissionEditVM
{
    public string RoleName { get; set; } = null!;
    public string? RoleDisplayName { get; set; }
    public List<int> SelectedPermissionIds { get; set; } = new();
    public IReadOnlyList<SystemPermission> Permissions { get; set; } = Array.Empty<SystemPermission>();
}
