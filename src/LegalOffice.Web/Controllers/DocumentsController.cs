using LegalOffice.Application.ViewModels;
using LegalOffice.Domain.Entities;
using LegalOffice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using LegalOffice.Web.Services;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class DocumentsController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _environment;
    private readonly IPermissionService _permissions;

    public DocumentsController(AppDbContext db, IWebHostEnvironment environment, IPermissionService permissions)
    {
        _db = db;
        _environment = environment;
        _permissions = permissions;
    }

    public async Task<IActionResult> Create(int caseId)
    {
        if (await IsCaseClosedAsync(caseId))
        {
            TempData["ToastError"] = "لا يمكن رفع مستندات على قضية مغلقة.";
            return RedirectToAction("Details", "Cases", new { id = caseId });
        }

        if (!await CanManageCaseAsync(caseId))
        {
            return Forbid();
        }

        var vm = new DocumentCreateEditVM { CaseId = caseId };
        await Fill(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DocumentCreateEditVM vm)
    {
        if (await IsCaseClosedAsync(vm.CaseId))
        {
            TempData["ToastError"] = "لا يمكن رفع مستندات على قضية مغلقة.";
            return RedirectToAction("Details", "Cases", new { id = vm.CaseId });
        }

        if (!await CanManageCaseAsync(vm.CaseId))
        {
            return Forbid();
        }

        var files = vm.Files?.Where(x => x is { Length: > 0 }).ToList() ?? new List<IFormFile>();
        if (files.Count == 0)
        {
            ModelState.AddModelError(nameof(vm.Files), "اختار ملف صالح أو أكثر.");
        }

        if (!ModelState.IsValid)
        {
            await Fill(vm);
            return View(vm);
        }

        var folder = Path.Combine(_environment.WebRootPath, "uploads", "cases", vm.CaseId.ToString());
        Directory.CreateDirectory(folder);

        foreach (var file in files)
        {
            var safeFileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
            var physicalPath = Path.Combine(folder, safeFileName);
            await using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("uploads", "cases", vm.CaseId.ToString(), safeFileName).Replace("\\", "/");

            _db.CaseDocuments.Add(new CaseDocument
            {
                CaseId = vm.CaseId,
                DocumentTypeId = vm.DocumentTypeId,
                FileName = file.FileName,
                FilePath = relativePath,
                Notes = vm.Notes,
                UploadedAt = DateTime.Now
            });
        }

        await _db.SaveChangesAsync();
        TempData["ToastSuccess"] = "تم رفع المستند بنجاح.";
        return RedirectToAction("Details", "Cases", new { id = vm.CaseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var doc = await _db.CaseDocuments.FirstOrDefaultAsync(x => x.Id == id);
        if (doc is null)
        {
            return NotFound();
        }

        if (await IsCaseClosedAsync(doc.CaseId))
        {
            TempData["ToastError"] = "لا يمكن حذف مرفقات قضية مغلقة.";
            return RedirectToAction("Details", "Cases", new { id = doc.CaseId });
        }

        if (!await CanManageCaseAsync(doc.CaseId))
        {
            return Forbid();
        }

        var physicalPath = Path.Combine(_environment.WebRootPath, doc.FilePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }

        _db.CaseDocuments.Remove(doc);
        await _db.SaveChangesAsync();

        TempData["ToastSuccess"] = "تم حذف المستند بنجاح.";
        return RedirectToAction("Details", "Cases", new { id = doc.CaseId });
    }

    private async Task Fill(DocumentCreateEditVM vm)
    {
        vm.DocumentTypes = await _db.Lookups
            .Where(x => x.Type == "DocumentType" && x.IsActive)
            .OrderBy(x => x.NameAr)
            .Select(x => new SelectListItem(x.NameAr, x.Id.ToString()))
            .ToListAsync();
    }

    private async Task<bool> CanManageCaseAsync(int caseId)
    {
        if (await _permissions.HasPermissionAsync(User, "Cases.Edit"))
        {
            return true;
        }

        var lawyerId = await GetCurrentLawyerIdAsync();
        if (lawyerId == null)
        {
            return false;
        }

        var editId = await GetLookupIdAsync("CaseAccessLevel", "Edit");
        var manageId = await GetLookupIdAsync("CaseAccessLevel", "Manage");
        return await _db.CaseLawyers.AnyAsync(x =>
            x.CaseId == caseId &&
            x.LawyerId == lawyerId.Value &&
            x.AccessLevelId >= editId &&
            x.AccessLevelId <= manageId);
    }

    private async Task<int?> GetCurrentLawyerIdAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        return await _db.Lawyers
            .Where(x => x.UserId == userId)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync();
    }

    private async Task<int> GetLookupIdAsync(string type, string nameEn)
    {
        var item = await _db.Lookups.FirstOrDefaultAsync(x => x.Type == type && x.NameEn == nameEn);
        return item?.Id ?? 0;
    }

    private async Task<bool> IsCaseClosedAsync(int caseId)
    {
        var closedId = await _db.Lookups
            .Where(x => x.Type == "CaseStatus" && x.NameEn == "Closed")
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        return closedId > 0 && await _db.Cases.AnyAsync(x => x.Id == caseId && x.CaseStatusId == closedId);
    }
}
