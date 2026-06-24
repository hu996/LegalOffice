using LegalOffice.Application.ViewModels;
using LegalOffice.Domain.Entities;
using LegalOffice.Infrastructure.Persistence;
using LegalOffice.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Security.Claims;

namespace LegalOffice.Web.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IPermissionService _permissions;

    public UsersController(
        AppDbContext db,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IPermissionService permissions)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _permissions = permissions;
    }

    public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
    {
        if (!await _permissions.HasPermissionAsync(User, "Users.View"))
        {
            return Forbid();
        }

        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 50);

        var query = _db.Users
            .Include(x => x.UserType)
            .Include(x => x.Department)
            .Include(x => x.Lawyer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.FullName.Contains(search) ||
                x.Email!.Contains(search) ||
                x.UserName!.Contains(search));
        }

        var totalCount = await query.CountAsync();
        var users = await query
            .OrderBy(x => x.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var rows = new List<UserIndexItemVM>();
        foreach (var user in users)
        {
            rows.Add(new UserIndexItemVM
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                UserTypeName = user.UserType?.NameAr,
                DepartmentName = user.Department?.NameAr,
                LawyerName = user.Lawyer?.FullName,
                IsActive = user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.UtcNow
            });
        }

        ViewBag.Search = search;
        return View(new PagedResult<UserIndexItemVM>
        {
            Items = rows,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }

    public async Task<IActionResult> Create()
    {
        if (!await _permissions.HasPermissionAsync(User, "Users.Create"))
        {
            return Forbid();
        }

        var vm = new UserCreateEditVM();
        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateEditVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "Users.Create"))
        {
            return Forbid();
        }

        NormalizeVm(vm);
        await ValidateUserAsync(vm);

        if (!ModelState.IsValid)
        {
            await FillLookupsAsync(vm);
            return View(vm);
        }

        var user = new ApplicationUser
        {
            FullName = vm.FullName,
            Email = vm.Email,
            UserName = vm.UserName,
            EmailConfirmed = true,
            UserTypeId = vm.UserTypeId,
            DepartmentId = vm.DepartmentId
        };

        var createResult = await _userManager.CreateAsync(user, vm.Password!);
        if (!createResult.Succeeded)
        {
            AddIdentityErrors(createResult);
            await FillLookupsAsync(vm);
            return View(vm);
        }

        await _userManager.AddToRoleAsync(user, vm.RoleName);

        if (vm.RoleName == "Lawyer" && vm.LawyerId.HasValue)
        {
            await AttachLawyerAsync(user, vm.LawyerId.Value);
        }

        TempData["ToastSuccess"] = "تم إنشاء المستخدم بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        if (!await _permissions.HasPermissionAsync(User, "Users.Edit"))
        {
            return Forbid();
        }

        var user = await _db.Users.Include(x => x.UserType).Include(x => x.Department).Include(x => x.Lawyer).FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        var roleName = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "";
        var vm = new UserCreateEditVM
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? "",
            UserName = user.UserName ?? "",
            UserTypeId = user.UserTypeId,
            DepartmentId = user.DepartmentId,
            RoleName = roleName,
            LawyerId = user.LawyerId,
            IsActive = user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.UtcNow
        };

        await FillLookupsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserCreateEditVM vm)
    {
        if (!await _permissions.HasPermissionAsync(User, "Users.Edit"))
        {
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(vm.Id))
        {
            return BadRequest();
        }

        NormalizeVm(vm);
        await ValidateUserAsync(vm, vm.Id);

        if (!ModelState.IsValid)
        {
            await FillLookupsAsync(vm);
            return View(vm);
        }

        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == vm.Id);
        if (user == null)
        {
            return NotFound();
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, vm.RoleName);

        user.FullName = vm.FullName;
        user.Email = vm.Email;
        user.UserName = vm.UserName;
        user.UserTypeId = vm.UserTypeId;
        user.DepartmentId = vm.DepartmentId;
        user.LockoutEnabled = true;
        user.LockoutEnd = vm.IsActive ? null : DateTimeOffset.MaxValue;

        if (vm.RoleName == "Lawyer" && vm.LawyerId.HasValue)
        {
            await AttachLawyerAsync(user, vm.LawyerId.Value);
        }
        else
        {
            await DetachLawyerAsync(user);
        }

        await _userManager.UpdateAsync(user);

        TempData["ToastSuccess"] = "تم تعديل المستخدم بنجاح.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(string id)
    {
        if (!await _permissions.HasPermissionAsync(User, "Users.ToggleStatus"))
        {
            return Forbid();
        }

        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        user.LockoutEnabled = true;
        user.LockoutEnd = user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.UtcNow
            ? DateTimeOffset.MaxValue
            : null;

        await _db.SaveChangesAsync();

        TempData["ToastSuccess"] = user.LockoutEnd == null ? "تم تفعيل المستخدم." : "تم تعطيل المستخدم.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(string id)
    {
        if (!await _permissions.HasPermissionAsync(User, "Users.Edit"))
        {
            return Forbid();
        }

        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        var tempPassword = GenerateTemporaryPassword();

        user.MustChangePassword = true;
        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, tempPassword);
        await _userManager.UpdateAsync(user);
        await _userManager.UpdateSecurityStampAsync(user);

        TempData["ToastSuccess"] = $"تمت إعادة تعيين كلمة المرور بنجاح. كلمة المرور المؤقتة: {tempPassword} — يجب تغييرها عند أول دخول.";
        return RedirectToAction(nameof(Index));
    }

    private void NormalizeVm(UserCreateEditVM vm)
    {
        vm.Email = vm.Email?.Trim() ?? string.Empty;
        vm.UserName = vm.UserName?.Trim() ?? string.Empty;
        vm.FullName = vm.FullName?.Trim() ?? string.Empty;
    }

    private async Task ValidateUserAsync(UserCreateEditVM vm, string? excludeUserId = null)
    {
        if (!await _roleManager.RoleExistsAsync(vm.RoleName))
        {
            ModelState.AddModelError(nameof(vm.RoleName), "الدور المختار غير موجود.");
        }

        var emailExists = await _db.Users.AnyAsync(x => x.Email == vm.Email && x.Id != excludeUserId);
        if (emailExists)
        {
            ModelState.AddModelError(nameof(vm.Email), "البريد الإلكتروني مستخدم من قبل.");
        }

        var userNameExists = await _db.Users.AnyAsync(x => x.UserName == vm.UserName && x.Id != excludeUserId);
        if (userNameExists)
        {
            ModelState.AddModelError(nameof(vm.UserName), "اسم المستخدم مستخدم من قبل.");
        }

        if (string.IsNullOrWhiteSpace(excludeUserId) && string.IsNullOrWhiteSpace(vm.Password))
        {
            ModelState.AddModelError(nameof(vm.Password), "كلمة المرور مطلوبة.");
        }

        if (!string.IsNullOrWhiteSpace(vm.Password) && vm.Password != vm.ConfirmPassword)
        {
            ModelState.AddModelError(nameof(vm.ConfirmPassword), "كلمتا المرور غير متطابقتين.");
        }

        if (string.Equals(vm.RoleName, "Lawyer", StringComparison.OrdinalIgnoreCase) && !vm.LawyerId.HasValue)
        {
            ModelState.AddModelError(nameof(vm.LawyerId), "لازم تختار المحامي المرتبط بالحساب.");
        }

        if (!string.Equals(vm.RoleName, "Lawyer", StringComparison.OrdinalIgnoreCase))
        {
            vm.LawyerId = null;
        }
    }

    private async Task AttachLawyerAsync(ApplicationUser user, int lawyerId)
    {
        var lawyer = await _db.Lawyers.FirstOrDefaultAsync(x => x.Id == lawyerId);
        if (lawyer == null)
        {
            return;
        }

        var previousLawyer = await _db.Lawyers.FirstOrDefaultAsync(x => x.UserId == user.Id);
        if (previousLawyer != null && previousLawyer.Id != lawyer.Id)
        {
            previousLawyer.UserId = null;
        }

        lawyer.UserId = user.Id;
        user.LawyerId = lawyer.Id;
        await _db.SaveChangesAsync();
    }

    private async Task DetachLawyerAsync(ApplicationUser user)
    {
        if (user.LawyerId.HasValue)
        {
            var lawyer = await _db.Lawyers.FirstOrDefaultAsync(x => x.Id == user.LawyerId.Value);
            if (lawyer != null)
            {
                lawyer.UserId = null;
            }
        }

        user.LawyerId = null;
    }

    private async Task FillLookupsAsync(UserCreateEditVM vm)
    {
        vm.Roles = await _roleManager.Roles
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name!, x.Name!))
            .ToListAsync();

        vm.UserTypes = await _db.Lookups
            .Where(x => x.Type == "UserType" && x.IsActive)
            .OrderBy(x => x.NameAr)
            .Select(x => new SelectListItem(x.NameAr, x.Id.ToString()))
            .ToListAsync();

        vm.Departments = await _db.Lookups
            .Where(x => x.Type == "Department" && x.IsActive)
            .OrderBy(x => x.NameAr)
            .Select(x => new SelectListItem(x.NameAr, x.Id.ToString()))
            .ToListAsync();

        vm.Lawyers = await _db.Lawyers
            .Where(x => x.IsActive)
            .OrderBy(x => x.FullName)
            .Select(x => new SelectListItem(x.FullName, x.Id.ToString()))
            .ToListAsync();
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    private static string GenerateTemporaryPassword()
    {
        var value = RandomNumberGenerator.GetInt32(100000, 1000000);
        return value.ToString();
    }
}
