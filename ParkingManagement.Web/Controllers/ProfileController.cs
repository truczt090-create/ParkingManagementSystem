using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Profile;

namespace ParkingManagement.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    // ============================================
    // XEM HỒ SƠ
    // URL: /Profile
    // ============================================
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var result = await _profileService.GetProfileAsync();

        if (result == null || !result.Success || result.Data == null)
        {
            TempData["Error"] =
                result?.Message ?? "Không thể tải thông tin hồ sơ";

            return RedirectToAction("Index", "Home");
        }

        ViewBag.SuccessMessage = TempData["SuccessMessage"];
        ViewBag.Error = TempData["Error"];

        return View(result.Data);
    }

    // ============================================
    // MỞ TRANG CẬP NHẬT HỒ SƠ
    // URL: /Profile/Edit
    // ============================================
    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var result = await _profileService.GetProfileAsync();

        if (result == null || !result.Success || result.Data == null)
        {
            TempData["Error"] =
                result?.Message ?? "Không thể tải thông tin hồ sơ";

            return RedirectToAction("Index");
        }

        var model = new UpdateProfileViewModel
        {
            FullName = result.Data.FullName,
            Phone = result.Data.Phone,
            BusinessName = result.Data.BusinessName,
            TaxCode = result.Data.TaxCode,
            Role = result.Data.Role,
            Email = result.Data.Email,
            AvatarUrl = result.Data.AvatarUrl
        };

        return View(model);
    }

    // ============================================
    // LƯU CẬP NHẬT HỒ SƠ
    // ============================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        UpdateProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result =
            await _profileService.UpdateProfileAsync(model);

        if (result == null ||
            !result.Success ||
            result.Data == null)
        {
            ModelState.AddModelError(
                "",
                result?.Message ?? "Cập nhật hồ sơ thất bại");

            return View(model);
        }

        // Cập nhật lại Session
        HttpContext.Session.SetString(
            "FullName",
            result.Data.FullName
        );

        HttpContext.Session.SetString(
            "Phone",
            result.Data.Phone ?? ""
        );

        HttpContext.Session.SetString(
            "AvatarUrl",
            result.Data.AvatarUrl
                ?? "/images/default-avatar.png"
        );

        TempData["SuccessMessage"] =
            "Cập nhật hồ sơ thành công";

        return RedirectToAction("Index");
    }

    // ============================================
    // MỞ TRANG ĐỔI MẬT KHẨU
    // URL: /Profile/ChangePassword
    // ============================================
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    // ============================================
    // LƯU MẬT KHẨU MỚI
    // ============================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result =
            await _profileService.ChangePasswordAsync(model);

        if (result == null || !result.Success)
        {
            ModelState.AddModelError(
                "",
                result?.Message ?? "Đổi mật khẩu thất bại");

            return View(model);
        }

        TempData["SuccessMessage"] =
            "Đổi mật khẩu thành công";

        return RedirectToAction("Index");
    }
}