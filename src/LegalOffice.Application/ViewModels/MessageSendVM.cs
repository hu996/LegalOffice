using Microsoft.AspNetCore.Mvc.Rendering;
namespace LegalOffice.Application.ViewModels;
public class MessageSendVM
{
    public int CaseId { get; set; }
    public int ClientId { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Channel { get; set; } = "WhatsApp";
    public int? TemplateId { get; set; }
    public string MessageText { get; set; } = null!;
    public IEnumerable<SelectListItem> Templates { get; set; } = new List<SelectListItem>();
}
