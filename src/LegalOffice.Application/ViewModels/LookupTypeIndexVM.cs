using LegalOffice.Domain.Entities;

namespace LegalOffice.Application.ViewModels;

public class LookupTypeIndexVM
{
    public PagedResult<LookupType> Types { get; set; } = new();
    public string? Search { get; set; }
}
