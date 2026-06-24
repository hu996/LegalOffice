using LegalOffice.Application.Interfaces;
using LegalOffice.Infrastructure.Persistence;
using LegalOffice.Domain.Entities;
namespace LegalOffice.Infrastructure.Services;
public class CaseTimelineService : ICaseTimelineService
{
    private readonly AppDbContext _db;
    public CaseTimelineService(AppDbContext db) => _db = db;
    public async Task AddAsync(int caseId, string title, string? description = null, string eventType = "General")
    {
        _db.CaseTimelines.Add(new CaseTimeline { CaseId = caseId, Title = title, Description = description, EventType = eventType });
        await _db.SaveChangesAsync();
    }
}
