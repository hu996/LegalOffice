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
public class ConsultationsController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;
    private readonly IWorkflowStatusService _workflowStatus;

    public ConsultationsController(AppDbContext db, IPermissionService permissions, IWorkflowStatusService workflowStatus)
    {
        _db = db;
        _permissions = permissions;
        _workflowStatus = workflowStatus;
    }

    public async Task<IActionResult> Index(string? search)
    {
        if (!await _permissions.HasPermissionAsync(User, "Consultations.View"))
        {
            return Forbid();
        }

        var query = _db.LegalConsultations
            .AsNoTracking()
            .Include(x => x.Client)
            .Include(x => x.AssignedLawyer)
            .Include(x => x.ConsultationTypeLookup)
            .Include(x => x.ConsultationStatusLookup)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.ConsultationNumber.Contains(search) || x.Title.Contains(search) || x.Client.FullName.Contains(search));
        }

        var items = await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        ViewBag.Search = search;
        return View(items);
    }

    public async Task<IActionResult> Create()
    {
        if (!await _permissions.HasPermissionAsync(User, "Consultations.Create"))
        {
            return Forbid();
        }

        var vm = new LegalConsultationVM { RequestDate = DateTime.Today, ConsultationStatusLookupId = await _workflowStatus.GetInitialStatusIdAsync("ConsultationStatus") ?? 0 };
        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LegalConsultationVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "Consultations.Create"))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            await Fill(vm);
            return View(vm);
        }

        if (!await _workflowStatus.ValidateSequentialTransitionAsync("ConsultationStatus", null, vm.ConsultationStatusLookupId, ModelState, nameof(vm.ConsultationStatusLookupId), "الاستشارة"))
        {
            await Fill(vm);
            return View(vm);
        }

        var entity = new LegalConsultation
        {
            ConsultationNumber = vm.ConsultationNumber.Trim(),
            Title = vm.Title.Trim(),
            ClientId = vm.ClientId,
            AssignedLawyerId = vm.AssignedLawyerId,
            ConsultationTypeLookupId = vm.ConsultationTypeLookupId,
            ConsultationStatusLookupId = vm.ConsultationStatusLookupId,
            RequestDate = vm.RequestDate,
            ResponseDate = vm.ResponseDate,
            ConsultationFees = vm.ConsultationFees,
            Subject = vm.Subject,
            LegalOpinion = vm.LegalOpinion,
            Notes = vm.Notes,
            BranchId = vm.BranchId,
            DepartmentId = vm.DepartmentId
        };

        _db.LegalConsultations.Add(entity);
        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تمت إضافة الاستشارة بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!await _permissions.HasPermissionAsync(User, "Consultations.Edit"))
        {
            return Forbid();
        }

        var entity = await _db.LegalConsultations.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        var vm = new LegalConsultationVM
        {
            Id = entity.Id,
            ConsultationNumber = entity.ConsultationNumber,
            Title = entity.Title,
            ClientId = entity.ClientId,
            AssignedLawyerId = entity.AssignedLawyerId,
            ConsultationTypeLookupId = entity.ConsultationTypeLookupId,
            ConsultationStatusLookupId = entity.ConsultationStatusLookupId,
            RequestDate = entity.RequestDate,
            ResponseDate = entity.ResponseDate,
            ConsultationFees = entity.ConsultationFees,
            Subject = entity.Subject,
            LegalOpinion = entity.LegalOpinion,
            Notes = entity.Notes,
            BranchId = entity.BranchId,
            DepartmentId = entity.DepartmentId
        };

        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(LegalConsultationVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "Consultations.Edit"))
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

        var existing = await _db.LegalConsultations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == vm.Id.Value);
        if (existing == null)
        {
            return NotFound();
        }

        if (!await _workflowStatus.ValidateSequentialTransitionAsync("ConsultationStatus", existing.ConsultationStatusLookupId, vm.ConsultationStatusLookupId, ModelState, nameof(vm.ConsultationStatusLookupId), "الاستشارة"))
        {
            await Fill(vm);
            return View(vm);
        }

        var entity = await _db.LegalConsultations.FirstOrDefaultAsync(x => x.Id == vm.Id.Value);
        if (entity == null)
        {
            return NotFound();
        }

        entity.ConsultationNumber = vm.ConsultationNumber.Trim();
        entity.Title = vm.Title.Trim();
        entity.ClientId = vm.ClientId;
        entity.AssignedLawyerId = vm.AssignedLawyerId;
        entity.ConsultationTypeLookupId = vm.ConsultationTypeLookupId;
        entity.ConsultationStatusLookupId = vm.ConsultationStatusLookupId;
        entity.RequestDate = vm.RequestDate;
        entity.ResponseDate = vm.ResponseDate;
        entity.ConsultationFees = vm.ConsultationFees;
        entity.Subject = vm.Subject;
        entity.LegalOpinion = vm.LegalOpinion;
        entity.Notes = vm.Notes;
        entity.BranchId = vm.BranchId;
        entity.DepartmentId = vm.DepartmentId;
        entity.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();

        TempData["ToastSuccess"] = "تم تعديل الاستشارة بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    private async Task Fill(LegalConsultationVM vm)
    {
        vm.Clients = await _db.Clients.OrderBy(x => x.FullName).Select(x => new SelectListItem(x.FullName, x.Id.ToString())).ToListAsync();
        vm.Lawyers = await _db.Lawyers.OrderBy(x => x.FullName).Select(x => new SelectListItem(x.FullName, x.Id.ToString())).ToListAsync();
        vm.Types = await SelectLookups("ConsultationType");
        vm.Branches = await _db.Branches.OrderBy(x => x.NameAr).Select(x => new SelectListItem(x.NameAr, x.Id.ToString())).ToListAsync();
        vm.Departments = await _db.Lookups.Where(x => x.Type == "Department" && x.IsActive).OrderBy(x => x.NameAr).Select(x => new SelectListItem(x.NameAr, x.Id.ToString())).ToListAsync();
        var currentStatusId = vm.Id.HasValue
            ? await _db.LegalConsultations.AsNoTracking().Where(x => x.Id == vm.Id.Value).Select(x => (int?)x.ConsultationStatusLookupId).FirstOrDefaultAsync()
            : null;
        vm.Statuses = await _workflowStatus.GetSequentialOptionsAsync("ConsultationStatus", currentStatusId);
    }

    private async Task<List<SelectListItem>> SelectLookups(string type)
    {
        return await _db.Lookups.Where(x => x.Type == type && x.IsActive).OrderBy(x => x.NameAr).Select(x => new SelectListItem(x.NameAr, x.Id.ToString())).ToListAsync();
    }
}
