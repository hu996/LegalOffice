using LegalOffice.Infrastructure.Persistence;
using LegalOffice.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class DepartmentsController : Controller
{
    private const string LookupTypeCode = "Department";
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;

    public DepartmentsController(AppDbContext db, IPermissionService permissions)
    {
        _db = db;
        _permissions = permissions;
    }

    public async Task<IActionResult> Index()
    {
        if (!await _permissions.HasPermissionAsync(User, "Departments.View"))
        {
            return Forbid();
        }

        var lookupType = await _db.LookupTypes.FirstOrDefaultAsync(x => x.Code == LookupTypeCode);
        if (lookupType == null)
        {
            return NotFound();
        }

        var items = await _db.Lookups
            .AsNoTracking()
            .Where(x => x.LookupTypeId == lookupType.Id)
            .OrderByDescending(x => x.IsActive)
            .ThenBy(x => x.NameAr)
            .ToListAsync();

        ViewBag.LookupType = lookupType;
        return View(items);
    }
}
