using LegalOffice.Application.ViewModels;
using LegalOffice.Domain.Entities;
using LegalOffice.Infrastructure.Persistence;
using LegalOffice.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class ContractsController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;

    public ContractsController(AppDbContext db, IPermissionService permissions)
    {
        _db = db;
        _permissions = permissions;
    }

    public async Task<IActionResult> Index(string? search)
    {
        if (!await _permissions.HasPermissionAsync(User, "Contracts.View"))
        {
            return Forbid();
        }

        var query = _db.Contracts
            .AsNoTracking()
            .Include(x => x.Client)
            .Include(x => x.AssignedLawyer)
            .Include(x => x.ContractTypeLookup)
            .Include(x => x.StatusLookup)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.ContractNumber.Contains(search) || x.Title.Contains(search) || x.Client.FullName.Contains(search));
        }

        var items = await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        ViewBag.Search = search;
        return View(items);
    }

    public async Task<IActionResult> Create()
    {
        if (!await _permissions.HasPermissionAsync(User, "Contracts.Create"))
        {
            return Forbid();
        }

        var vm = new ContractVM { StartDate = DateTime.Today };
        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContractVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "Contracts.Create"))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            await Fill(vm);
            return View(vm);
        }

        _db.Contracts.Add(new Contract
        {
            ContractNumber = vm.ContractNumber.Trim(),
            Title = vm.Title.Trim(),
            ContractTypeLookupId = vm.ContractTypeLookupId!.Value,
            ClientId = vm.ClientId!.Value,
            AssignedLawyerId = vm.AssignedLawyerId!.Value,
            StartDate = vm.StartDate,
            EndDate = vm.EndDate,
            ContractValue = vm.ContractValue!.Value,
            StatusLookupId = vm.StatusLookupId!.Value,
            Notes = vm.Notes,
            BranchId = vm.BranchId,
            DepartmentId = vm.DepartmentId
        });

        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تمت إضافة العقد بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!await _permissions.HasPermissionAsync(User, "Contracts.Edit"))
        {
            return Forbid();
        }

        var entity = await _db.Contracts.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        var vm = new ContractVM
        {
            Id = entity.Id,
            ContractNumber = entity.ContractNumber,
            Title = entity.Title,
            ContractTypeLookupId = entity.ContractTypeLookupId,
            ClientId = entity.ClientId,
            AssignedLawyerId = entity.AssignedLawyerId,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            ContractValue = entity.ContractValue,
            StatusLookupId = entity.StatusLookupId,
            Notes = entity.Notes,
            BranchId = entity.BranchId,
            DepartmentId = entity.DepartmentId
        };

        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ContractVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "Contracts.Edit"))
        {
            return Forbid();
        }

        if (!vm.Id.HasValue)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await Fill(vm);
            return View(vm);
        }

        var entity = await _db.Contracts.FirstOrDefaultAsync(x => x.Id == vm.Id.Value);
        if (entity == null)
        {
            return NotFound();
        }

        entity.ContractNumber = vm.ContractNumber.Trim();
        entity.Title = vm.Title.Trim();
        entity.ContractTypeLookupId = vm.ContractTypeLookupId!.Value;
        entity.ClientId = vm.ClientId!.Value;
        entity.AssignedLawyerId = vm.AssignedLawyerId!.Value;
        entity.StartDate = vm.StartDate;
        entity.EndDate = vm.EndDate;
        entity.ContractValue = vm.ContractValue!.Value;
        entity.StatusLookupId = vm.StatusLookupId!.Value;
        entity.Notes = vm.Notes;
        entity.BranchId = vm.BranchId;
        entity.DepartmentId = vm.DepartmentId;
        await _db.SaveChangesAsync();

        TempData["ToastSuccess"] = "تم تعديل العقد بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    private async Task Fill(ContractVM vm)
    {
        vm.Clients = await _db.Clients.OrderBy(x => x.FullName).Select(x => new SelectListItem(x.FullName, x.Id.ToString())).ToListAsync();
        vm.Lawyers = await _db.Lawyers.OrderBy(x => x.FullName).Select(x => new SelectListItem(x.FullName, x.Id.ToString())).ToListAsync();
        vm.Types = await SelectLookups("ContractType");
        vm.Statuses = await SelectLookups("ContractStatus");
        vm.Branches = await _db.Branches.OrderBy(x => x.NameAr).Select(x => new SelectListItem(x.NameAr, x.Id.ToString())).ToListAsync();
        vm.Departments = await _db.Lookups.Where(x => x.Type == "Department" && x.IsActive).OrderBy(x => x.NameAr).Select(x => new SelectListItem(x.NameAr, x.Id.ToString())).ToListAsync();
    }

    private async Task<List<SelectListItem>> SelectLookups(string type)
    {
        return await _db.Lookups.Where(x => x.Type == type && x.IsActive).OrderBy(x => x.NameAr).Select(x => new SelectListItem(x.NameAr, x.Id.ToString())).ToListAsync();
    }
}
