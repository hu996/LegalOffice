using LegalOffice.Application.ViewModels;
using LegalOffice.Domain.Entities;
using LegalOffice.Infrastructure.Persistence;
using LegalOffice.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class PowerOfAttorneyController : Controller
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;
    private readonly IWebHostEnvironment _environment;
    private readonly IWorkflowStatusService _workflowStatus;

    public PowerOfAttorneyController(AppDbContext db, IPermissionService permissions, IWebHostEnvironment environment, IWorkflowStatusService workflowStatus)
    {
        _db = db;
        _permissions = permissions;
        _environment = environment;
        _workflowStatus = workflowStatus;
    }

    public async Task<IActionResult> Index(string? search)
    {
        if (!await _permissions.HasPermissionAsync(User, "PowerOfAttorney.View"))
        {
            return Forbid();
        }

        var query = _db.PowerOfAttorneys
            .AsNoTracking()
            .Include(x => x.Client)
            .Include(x => x.TypeLookup)
            .Include(x => x.StatusLookup)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.PowerNumber.Contains(search) || x.Client.FullName.Contains(search));
        }

        return View(await query.OrderByDescending(x => x.CreatedAt).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        if (!await _permissions.HasPermissionAsync(User, "PowerOfAttorney.View"))
        {
            return Forbid();
        }

        var item = await _db.PowerOfAttorneys
            .AsNoTracking()
            .Include(x => x.Client)
            .Include(x => x.TypeLookup)
            .Include(x => x.StatusLookup)
            .FirstOrDefaultAsync(x => x.Id == id);

        return item is null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!await _permissions.HasPermissionAsync(User, "PowerOfAttorney.Edit"))
        {
            return Forbid();
        }

        var item = await _db.PowerOfAttorneys.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (item is null)
        {
            return NotFound();
        }

        var vm = new PowerOfAttorneyVM
        {
            Id = item.Id,
            PowerNumber = item.PowerNumber,
            ClientId = item.ClientId,
            TypeLookupId = item.TypeLookupId,
            IssueDate = item.IssueDate,
            ExpiryDate = item.ExpiryDate,
            RegistrationOffice = item.RegistrationOffice,
            Notes = item.Notes,
            StatusLookupId = item.StatusLookupId,
            CaseId = item.CaseId
        };

        ViewBag.ExistingAttachments = GetAttachmentPaths(item.FilePath);
        await Fill(vm);
        return View(vm);
    }

    public async Task<IActionResult> Create()
    {
        if (!await _permissions.HasPermissionAsync(User, "PowerOfAttorney.Create"))
        {
            return Forbid();
        }

        var vm = new PowerOfAttorneyVM { IssueDate = DateTime.Today, StatusLookupId = await _workflowStatus.GetInitialStatusIdAsync("PowerStatus") ?? 0 };
        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PowerOfAttorneyVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "PowerOfAttorney.Create"))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            await Fill(vm);
            return View(vm);
        }

        if (!await _workflowStatus.ValidateSequentialTransitionAsync("PowerStatus", null, vm.StatusLookupId!.Value, ModelState, nameof(vm.StatusLookupId), "التوكيل"))
        {
            await Fill(vm);
            return View(vm);
        }

        if (vm.Attachments is null || vm.Attachments.Count == 0 || vm.Attachments.All(x => x.Length == 0))
        {
            ModelState.AddModelError(nameof(vm.Attachments), "يجب رفع مرفق واحد على الأقل.");
            await Fill(vm);
            return View(vm);
        }

        _db.PowerOfAttorneys.Add(new PowerOfAttorney
        {
            PowerNumber = vm.PowerNumber.Trim(),
            ClientId = vm.ClientId!.Value,
            TypeLookupId = vm.TypeLookupId!.Value,
            IssueDate = vm.IssueDate,
            ExpiryDate = vm.ExpiryDate,
            RegistrationOffice = vm.RegistrationOffice,
            Notes = vm.Notes,
            FilePath = await SaveAttachmentsAsync(vm.Attachments!),
            StatusLookupId = vm.StatusLookupId!.Value,
            CaseId = vm.CaseId
        });

        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تمت إضافة التوكيل بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PowerOfAttorneyVM vm, string[]? removeAttachments)
    {
        if (!await _permissions.HasPermissionAsync(User, "PowerOfAttorney.Edit"))
        {
            return Forbid();
        }

        var item = await _db.PowerOfAttorneys.FirstOrDefaultAsync(x => x.Id == vm.Id);
        if (item is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.ExistingAttachments = GetAttachmentPaths(item.FilePath);
            await Fill(vm);
            return View(vm);
        }

        if (!await _workflowStatus.ValidateSequentialTransitionAsync("PowerStatus", item.StatusLookupId, vm.StatusLookupId!.Value, ModelState, nameof(vm.StatusLookupId), "التوكيل"))
        {
            ViewBag.ExistingAttachments = GetAttachmentPaths(item.FilePath);
            await Fill(vm);
            return View(vm);
        }

        var existingAttachments = GetAttachmentPaths(item.FilePath).ToList();
        if (removeAttachments is { Length: > 0 })
        {
            var toRemove = removeAttachments.Where(x => !string.IsNullOrWhiteSpace(x)).ToHashSet(StringComparer.OrdinalIgnoreCase);
            existingAttachments = existingAttachments.Where(x => !toRemove.Contains(x)).ToList();
            foreach (var relativePath in toRemove)
            {
                var physical = Path.Combine(_environment.WebRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(physical))
                {
                    System.IO.File.Delete(physical);
                }
            }
        }

        var newAttachments = await SaveAttachmentsAsync(vm.Attachments ?? new List<IFormFile>());
        if (!string.IsNullOrWhiteSpace(newAttachments))
        {
            existingAttachments.AddRange(newAttachments.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        }

        if (existingAttachments.Count == 0)
        {
            ModelState.AddModelError(nameof(vm.Attachments), "يجب الاحتفاظ بمرفق واحد على الأقل.");
            ViewBag.ExistingAttachments = GetAttachmentPaths(item.FilePath);
            await Fill(vm);
            return View(vm);
        }

        item.PowerNumber = vm.PowerNumber.Trim();
        item.ClientId = vm.ClientId!.Value;
        item.TypeLookupId = vm.TypeLookupId!.Value;
        item.IssueDate = vm.IssueDate;
        item.ExpiryDate = vm.ExpiryDate;
        item.RegistrationOffice = vm.RegistrationOffice;
        item.Notes = vm.Notes;
        item.FilePath = string.Join("|", existingAttachments);
        item.StatusLookupId = vm.StatusLookupId!.Value;
        item.CaseId = vm.CaseId;

        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تم تحديث التوكيل بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    private async Task Fill(PowerOfAttorneyVM vm)
    {
        vm.Clients = await _db.Clients.OrderBy(x => x.FullName).Select(x => new SelectListItem(x.FullName, x.Id.ToString())).ToListAsync();
        vm.Types = await SelectLookups("PowerType");
        var currentStatusId = vm.Id.HasValue
            ? await _db.PowerOfAttorneys.AsNoTracking().Where(x => x.Id == vm.Id.Value).Select(x => (int?)x.StatusLookupId).FirstOrDefaultAsync()
            : null;
        vm.Statuses = await _workflowStatus.GetSequentialOptionsAsync("PowerStatus", currentStatusId);
        vm.Cases = await _db.Cases.OrderByDescending(x => x.Id).Select(x => new SelectListItem($"{x.CaseNumber} - {x.Title}", x.Id.ToString())).ToListAsync();
    }

    private async Task<List<SelectListItem>> SelectLookups(string type)
    {
        return await _db.Lookups.Where(x => x.Type == type && x.IsActive).OrderBy(x => x.NameAr).Select(x => new SelectListItem(x.NameAr, x.Id.ToString())).ToListAsync();
    }

    private async Task<string?> SaveAttachmentsAsync(List<IFormFile> attachments)
    {
        var validFiles = attachments.Where(x => x is { Length: > 0 }).ToList();
        if (validFiles.Count == 0)
        {
            return null;
        }

        var folder = Path.Combine(_environment.WebRootPath, "uploads", "power-of-attorneys");
        Directory.CreateDirectory(folder);

        var savedPaths = new List<string>();

        foreach (var file in validFiles)
        {
            var safeFileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
            var physicalPath = Path.Combine(folder, safeFileName);

            await using var stream = new FileStream(physicalPath, FileMode.Create);
            await file.CopyToAsync(stream);

            savedPaths.Add(Path.Combine("uploads", "power-of-attorneys", safeFileName).Replace("\\", "/"));
        }

        return string.Join("|", savedPaths);
    }

    public static IReadOnlyList<string> GetAttachmentPaths(string? storedValue)
    {
        if (string.IsNullOrWhiteSpace(storedValue))
        {
            return Array.Empty<string>();
        }

        if (storedValue.Contains('|'))
        {
            return storedValue.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        return new[] { storedValue };
    }

    public static string GetAttachmentName(string path) => Path.GetFileName(path);
}
