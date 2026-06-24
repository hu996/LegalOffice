using LegalOffice.Application.ViewModels;
using LegalOffice.Infrastructure.Persistence;
using LegalOffice.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class AuditLogsController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;

    public AuditLogsController(AppDbContext db, IPermissionService permissions)
    {
        _db = db;
        _permissions = permissions;
    }

    public async Task<IActionResult> Index(string? userName, string? entityName, string? actionType, DateTime? fromDate, DateTime? toDate)
    {
        if (!await _permissions.HasPermissionAsync(User, "AuditLogs.View"))
        {
            return Forbid();
        }

        var query = _db.AuditLogs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(userName))
        {
            query = query.Where(x => x.UserName != null && x.UserName.Contains(userName));
        }

        if (!string.IsNullOrWhiteSpace(entityName))
        {
            query = query.Where(x => x.EntityName.Contains(entityName));
        }

        if (!string.IsNullOrWhiteSpace(actionType))
        {
            query = query.Where(x => x.ActionType.Contains(actionType));
        }

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt < toDate.Value.AddDays(1));
        }

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Take(500)
            .Select(x => new AuditLogListItemVM
            {
                Id = x.Id,
                UserName = x.UserName,
                ActionType = x.ActionType,
                EntityName = x.EntityName,
                EntityId = x.EntityId,
                Description = x.Description,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        ViewBag.UserName = userName;
        ViewBag.EntityName = entityName;
        ViewBag.ActionType = actionType;
        ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
        ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
        return View(items);
    }
}
