using LegalOffice.Infrastructure.Persistence;
using LegalOffice.Domain.Entities;
using LegalOffice.Application.Interfaces;
using LegalOffice.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace LegalOffice.Web.Controllers;
[Authorize]
public class MessagesController : Controller
{
    private readonly AppDbContext _db; private readonly IMessageService _messages; private readonly ICaseTimelineService _timeline;
    public MessagesController(AppDbContext db,IMessageService messages,ICaseTimelineService timeline){_db=db;_messages=messages;_timeline=timeline;}
    public async Task<IActionResult> Send(int caseId)
    {
        var c=await _db.Cases.Include(x=>x.Client).Include(x=>x.CaseStatus).FirstOrDefaultAsync(x=>x.Id==caseId); if(c==null)return NotFound();
        var vm=new MessageSendVM{CaseId=caseId,ClientId=c.ClientId,PhoneNumber=c.Client.WhatsAppPhone??c.Client.Phone??"",MessageText=$"عميلنا العزيز {c.Client.FullName}، حالة القضية رقم {c.CaseNumber}: {c.CaseStatus.NameAr}"};
        await Fill(vm); return View(vm);
    }
    [HttpPost] public async Task<IActionResult> Send(MessageSendVM vm)
    {
        if(string.IsNullOrWhiteSpace(vm.PhoneNumber)||string.IsNullOrWhiteSpace(vm.MessageText)){await Fill(vm);return View(vm);} 
        var result = vm.Channel=="SMS" ? await _messages.SendSmsAsync(vm.PhoneNumber,vm.MessageText) : await _messages.SendWhatsAppAsync(vm.PhoneNumber,vm.MessageText);
        _db.MessageLogs.Add(new MessageLog{CaseId=vm.CaseId,ClientId=vm.ClientId,Channel=vm.Channel,PhoneNumber=vm.PhoneNumber,MessageText=vm.MessageText,IsSent=result.success,ProviderResponse=result.response});
        await _db.SaveChangesAsync(); await _timeline.AddAsync(vm.CaseId,$"تم إرسال {vm.Channel}",vm.MessageText,"MessageSent");
        return RedirectToAction("Details","Cases",new{id=vm.CaseId});
    }
    private async Task Fill(MessageSendVM vm){ vm.Templates=await _db.MessageTemplates.Where(x=>x.IsActive).Select(x=>new SelectListItem(x.Name,x.Id.ToString())).ToListAsync(); }
}
