namespace LegalOffice.Application.ViewModels;

public class PaginationVM
{
    public string ActionName { get; set; } = "Index";
    public string? ControllerName { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public IDictionary<string, object?> RouteValues { get; set; } = new Dictionary<string, object?>();
}
