using LegalOffice.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class BackupController : Controller
{
    private readonly IPermissionService _permissions;

    public BackupController(IPermissionService permissions) => _permissions = permissions;

    public async Task<IActionResult> Index()
    {
        if (!await _permissions.HasPermissionAsync(User, "Backup.View"))
        {
            return Forbid();
        }

        return View();
    }
}
