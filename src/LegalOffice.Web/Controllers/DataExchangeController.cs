using ClosedXML.Excel;
using LegalOffice.Infrastructure.Persistence;
using LegalOffice.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class DataExchangeController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;

    public DataExchangeController(AppDbContext db, IPermissionService permissions)
    {
        _db = db;
        _permissions = permissions;
    }

    public async Task<IActionResult> Index()
    {
        if (!await _permissions.HasPermissionAsync(User, "ImportExport.View"))
        {
            return Forbid();
        }

        return View();
    }

    public async Task<IActionResult> ExportCases()
    {
        if (!await _permissions.HasPermissionAsync(User, "ImportExport.View"))
        {
            return Forbid();
        }

        var data = await _db.Cases.AsNoTracking().Include(x => x.Client).OrderByDescending(x => x.CreatedAt).ToListAsync();
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("القضايا");
        ws.Cell(1, 1).Value = "رقم القضية";
        ws.Cell(1, 2).Value = "عنوان القضية";
        ws.Cell(1, 3).Value = "العميل";
        ws.Cell(1, 4).Value = "تاريخ البدء";
        ws.Cell(1, 5).Value = "تاريخ الإغلاق";

        for (var i = 0; i < data.Count; i++)
        {
            var row = i + 2;
            ws.Cell(row, 1).Value = data[i].CaseNumber;
            ws.Cell(row, 2).Value = data[i].Title;
            ws.Cell(row, 3).Value = data[i].Client.FullName;
            ws.Cell(row, 4).Value = data[i].StartDate;
            ws.Cell(row, 5).Value = data[i].ClosedDate;
        }

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Cases.xlsx");
    }

    public async Task<IActionResult> ExportClients()
    {
        if (!await _permissions.HasPermissionAsync(User, "ImportExport.View"))
        {
            return Forbid();
        }

        var data = await _db.Clients.AsNoTracking().OrderBy(x => x.FullName).ToListAsync();
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("العملاء");
        ws.Cell(1, 1).Value = "الاسم";
        ws.Cell(1, 2).Value = "الرقم القومي";
        ws.Cell(1, 3).Value = "الهاتف";
        ws.Cell(1, 4).Value = "العنوان";

        for (var i = 0; i < data.Count; i++)
        {
            var row = i + 2;
            ws.Cell(row, 1).Value = data[i].FullName;
            ws.Cell(row, 2).Value = data[i].NationalId;
            ws.Cell(row, 3).Value = data[i].Phone;
            ws.Cell(row, 4).Value = data[i].Address;
        }

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Clients.xlsx");
    }
}
