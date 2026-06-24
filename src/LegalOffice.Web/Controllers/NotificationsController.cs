using LegalOffice.Application.ViewModels;
using LegalOffice.Domain.Entities;
using LegalOffice.Infrastructure.Persistence;
using LegalOffice.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class NotificationsController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;

    public NotificationsController(AppDbContext db, IPermissionService permissions)
    {
        _db = db;
        _permissions = permissions;
    }

    public async Task<IActionResult> Index()
    {
        if (!await _permissions.HasPermissionAsync(User, "Notifications.View"))
        {
            return Forbid();
        }

        var lawyerId = await GetCurrentLawyerIdAsync();
        if (lawyerId == null)
        {
            return View(new NotificationCenterVM());
        }

        var items = await _db.Notifications
            .AsNoTracking()
            .Where(x => x.LawyerId == lawyerId.Value)
            .OrderByDescending(x => x.CreatedAt)
            .Take(100)
            .Select(x => new NotificationItemVM
            {
                Id = x.Id,
                Title = x.Title,
                Body = x.Body,
                TargetUrl = x.TargetUrl,
                CreatedAt = x.CreatedAt,
                IsRead = x.IsRead
            })
            .ToListAsync();

        return View(new NotificationCenterVM
        {
            Items = items,
            UnreadCount = items.Count(x => !x.IsRead)
        });
    }

    public async Task<IActionResult> Open(int id)
    {
        if (!await _permissions.HasPermissionAsync(User, "Notifications.Open"))
        {
            return Forbid();
        }

        var lawyerId = await GetCurrentLawyerIdAsync();
        if (lawyerId == null)
        {
            return Forbid();
        }

        var notification = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.LawyerId == lawyerId.Value);
        if (notification == null)
        {
            return NotFound();
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.Now;
            await _db.SaveChangesAsync();
        }

        if (!string.IsNullOrWhiteSpace(notification.TargetUrl))
        {
            return LocalRedirect(notification.TargetUrl);
        }

        if (notification.CaseId.HasValue)
        {
            return RedirectToAction("Details", "Cases", new { id = notification.CaseId.Value });
        }

        return RedirectToAction("Index", "Dashboard");
    }

    private async Task<int?> GetCurrentLawyerIdAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        return await _db.Lawyers
            .Where(x => x.UserId == userId)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync();
    }
}

public class NotificationCenterVM
{
    public int UnreadCount { get; set; }
    public IReadOnlyList<NotificationItemVM> Items { get; set; } = Array.Empty<NotificationItemVM>();
}
