using LegalOffice.Application.ViewModels;
using LegalOffice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LegalOffice.Web.ViewComponents;

public class NotificationsBellViewComponent : ViewComponent
{
    private readonly AppDbContext _db;

    public NotificationsBellViewComponent(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return View(new NotificationBellVM());
        }

        var lawyerId = await _db.Lawyers
            .Where(x => x.UserId == userId)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync();

        if (lawyerId == null)
        {
            return View(new NotificationBellVM());
        }

        var items = await _db.Notifications
            .AsNoTracking()
            .Where(x => x.LawyerId == lawyerId.Value)
            .OrderByDescending(x => x.CreatedAt)
            .Take(5)
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

        return View(new NotificationBellVM
        {
            UnreadCount = items.Count(x => !x.IsRead),
            Items = items
        });
    }
}
