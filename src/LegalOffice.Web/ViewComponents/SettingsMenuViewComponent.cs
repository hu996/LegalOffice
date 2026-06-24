using LegalOffice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalOffice.Web.ViewComponents;

public class SettingsMenuViewComponent : ViewComponent
{
    private readonly AppDbContext _db;

    public SettingsMenuViewComponent(AppDbContext db) => _db = db;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var lookupTypes = await _db.LookupTypes
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.NameAr)
            .ToListAsync();

        return View(lookupTypes);
    }
}
