namespace LegalOffice.Application.Interfaces;
public interface ICaseTimelineService
{
    Task AddAsync(int caseId, string title, string? description = null, string eventType = "General");
}
