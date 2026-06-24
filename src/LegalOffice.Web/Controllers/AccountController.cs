using LegalOffice.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
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
            ViewBag.Error = "الحساب غير نشط حالياً";
            return View();
        }

        if (user != null)
        {
            var result = await _signIn.CheckPasswordSignInAsync(user, password, true);
            if (result.Succeeded)
            {
                await _signIn.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        ViewBag.Error = "بيانات الدخول غير صحيحة";
        return View();
    }

    [AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied() => View();
}
