using LegalOffice.Domain.Entities;

namespace LegalOffice.Application.ViewModels;

public class LookupItemIndexVM
{
    public LookupType LookupType { get; set; } = null!;
    public PagedResult<Lookup> Items { get; set; } = new();
}
