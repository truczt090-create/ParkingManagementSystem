using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Auth;

namespace ParkingManagement.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.LoginAsync(model);

        if (result == null || !result.Success || result.Data == null)
        {
            ModelState.AddModelError(
                "",
                result?.Message ?? "Email hoặc mật khẩu không đúng"
            );

            return View(model);
        }


        // ============================================
        // 1. LƯU JWT VÀ THÔNG TIN VÀO SESSION
        // ============================================

        HttpContext.Session.SetString(
            "Token",
            result.Data.Token
        );

        HttpContext.Session.SetString(
            "Role",
            result.Data.Role
        );

        HttpContext.Session.SetString(
            "FullName",
            result.Data.FullName
        );

        HttpContext.Session.SetString("AvatarUrl", result.Data.AvatarUrl ?? "/images/default-avatar.png");

        HttpContext.Session.SetString(
            "Phone",
            result.Data.Phone ?? ""
);
        // ============================================
        // 2. TẠO CLAIMS CHO COOKIE AUTHENTICATION
        // ============================================

        var claims = new List<Claim>
    {
        new Claim(
            ClaimTypes.Name,
            result.Data.FullName
        ),

        new Claim(
            ClaimTypes.Email,
            model.Email
        ),

        new Claim(
            ClaimTypes.Role,
            result.Data.Role
        )
    };


        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );


        var principal = new ClaimsPrincipal(identity);


        // ============================================
        // 3. ĐĂNG NHẬP COOKIE
        // ============================================

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true
            }
        );


        // ============================================
        // 4. THÔNG BÁO LOGIN THÀNH CÔNG
        // ============================================

        TempData["LoginSuccess"] =
            "Đăng nhập thành công";


        // ============================================
        // 5. REDIRECT THEO ROLE
        // ============================================

        return result.Data.Role switch
        {
            "Customer" =>
                RedirectToAction(
                    "Index",
                    "Home"
                ),

            "Employee" =>
                RedirectToAction(
                    "CheckIn",
                    "Employee"
                ),

            "Owner" =>
                RedirectToAction(
                    "Dashboard",
                    "Owner"
                ),

            "Admin" =>
                RedirectToAction(
                    "Dashboard",
                    "Admin"
                ),

            _ =>
                RedirectToAction(
                    "Index",
                    "Home"
                )
        };
    }


    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _authService.RegisterAsync(model);

        if (result == null || !result.Success)
        {
            ModelState.AddModelError("", result?.Message ?? "Đăng ký thất bại");
            return View(model);
        }

        return RedirectToAction("Login");
    }
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        // Xóa Cookie Authentication
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        // Xóa toàn bộ Session
        HttpContext.Session.Clear();

        return RedirectToAction(
            "Index",
            "Home"
        );
    }
    [HttpGet]
    public IActionResult RegisterOwner() => View();

    [HttpPost]
    public async Task<IActionResult> RegisterOwner(RegisterOwnerViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _authService.RegisterOwnerAsync(model);

        if (result == null || !result.Success)
        {
            ModelState.AddModelError("", result?.Message ?? "Đăng ký thất bại");
            return View(model);
        }

        TempData["SuccessMessage"] = "Đăng ký chủ bãi xe thành công, vui lòng đăng nhập";
        return RedirectToAction("Login");
    }
}
