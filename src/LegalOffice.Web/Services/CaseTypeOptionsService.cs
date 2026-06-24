using LegalOffice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LegalOffice.Web.Services;

public class CaseTypeOptionsService : ICaseTypeOptionsService
{
    private readonly AppDbContext _db;

    public CaseTypeOptionsService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<SelectListItem>> GetVisibleCaseTypesAsync(ClaimsPrincipal user)
    {
        var allTypes = _db.Lookups.AsNoTracking()
            .Where(x => x.Type == "CaseType" && x.IsActive)
            .OrderBy(x => x.NameAr);

        if (user.Identity?.IsAuthenticated != true)
        {
            return await allTypes
                .Select(x => new SelectListItem(x.NameAr, x.Id.ToString()))
                .ToListAsync();
        }

        var roles = user.Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value)
            .Distinct()
            .ToList();

        if (roles.Contains("Admin") || !roles.Contains("Lawyer"))
        {
            return await allTypes
                .Select(x => new SelectListItem(x.NameAr, x.Id.ToString()))
                .ToListAsync();
        }

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return new List<SelectListItem>();
        }

        var allowedTypeIds = await _db.Lawyers.AsNoTracking()
            .Where(x => x.UserId == userId)
            .SelectMany(x => x.Specialties.Select(s => s.CaseTypeId))
            .Distinct()
            .ToListAsync();

        return await allTypes
            .Where(x => allowedTypeIds.Contains(x.Id))
            .Select(x => new SelectListItem(x.NameAr, x.Id.ToString()))
            .ToListAsync();
    }
}
