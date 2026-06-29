using LegalOffice.Domain.Entities;
using LegalOffice.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LegalOffice.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signIn;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signIn)
    {
        _userManager = userManager;
        _signIn = signIn;
    }

    [AllowAnonymous]
    public IActionResult Login() => View();

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email) ?? await _userManager.FindByNameAsync(email);
        if (user != null && user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow)
        {
            ModelState.AddModelError(nameof(email), "الحساب غير نشط حاليًا");
            ModelState.AddModelError(nameof(password), "الحساب غير نشط حاليًا");
            return View();
        }

        if (user != null)
        {
            var result = await _signIn.CheckPasswordSignInAsync(user, password, true);
            if (result.Succeeded)
            {
                await _signIn.SignInAsync(user, isPersistent: false);
                if (user.MustChangePassword)
                {
                    return RedirectToAction(nameof(ChangePassword), new { forced = true });
                }
                return RedirectToAction("Index", "Dashboard");
            }
        }

        ModelState.AddModelError(nameof(email), "بيانات الدخول غير صحيحة");
        ModelState.AddModelError(nameof(password), "بيانات الدخول غير صحيحة");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Login");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ChangePassword(bool forced = false)
    {
        if (!forced)
        {
            var user = await _userManager.GetUserAsync(User);
            forced = user?.MustChangePassword == true;
        }

        return View(new ChangePasswordVM { IsForced = forced });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordVM vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        if (user.MustChangePassword)
        {
            vm.IsForced = true;
        }

        if (!vm.IsForced && string.IsNullOrWhiteSpace(vm.CurrentPassword))
        {
            ModelState.AddModelError(nameof(vm.CurrentPassword), "كلمة المرور الحالية مطلوبة.");
            return View(vm);
        }

        IdentityResult result;
        if (vm.IsForced)
        {
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, vm.NewPassword);
            user.MustChangePassword = false;
            result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
            }
        }
        else
        {
            result = await _userManager.ChangePasswordAsync(user, vm.CurrentPassword, vm.NewPassword);
        }

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                if (string.Equals(error.Code, "PasswordMismatch", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(nameof(vm.CurrentPassword), error.Description);
                }
                else
                {
                    ModelState.AddModelError(nameof(vm.NewPassword), error.Description);
                }
            }

            return View(vm);
        }

        await _signIn.RefreshSignInAsync(user);
        TempData["ToastSuccess"] = "تم تغيير كلمة المرور بنجاح.";
        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult AccessDenied() => View();
}
