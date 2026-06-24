using LegalOffice.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LegalOffice.Web.ViewComponents;

public class SidebarMenuViewComponent : ViewComponent
{
    private readonly IPermissionService _permissions;

    public SidebarMenuViewComponent(IPermissionService permissions) => _permissions = permissions;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var groups = await _permissions.GetSidebarGroupsAsync(HttpContext.User);
        return View(groups);
    }
}
