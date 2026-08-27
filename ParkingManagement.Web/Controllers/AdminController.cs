using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Web.Services.Interfaces;

namespace ParkingManagement.Web.Controllers;

public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    public AdminController(IAdminService adminService) => _adminService = adminService;

    private bool RequireAdmin(out IActionResult? redirect)
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
        {
            redirect = RedirectToAction("Login", "Account");
            return false;
        }
        redirect = null;
        return true;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        if (!RequireAdmin(out var redirect)) return redirect!;

        var result = await _adminService.GetDashboardAsync();
        return View(result?.Data ?? new());
    }

    [HttpGet]
    public async Task<IActionResult> PendingLots(string status = "Pending")
    {
        if (!RequireAdmin(out var redirect)) return redirect!;

        var result = await _adminService.GetLotsByStatusAsync(status);
        ViewBag.Status = status;
        ViewBag.SuccessMessage = TempData["SuccessMessage"];

        return View(result?.Data ?? new());
    }

    [HttpPost]
    public async Task<IActionResult> Approve(int id)
    {
        if (!RequireAdmin(out var redirect)) return redirect!;

        await _adminService.ApproveLotAsync(id);
        TempData["SuccessMessage"] = "Đã duyệt bãi xe";
        return RedirectToAction("PendingLots");
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int id)
    {
        if (!RequireAdmin(out var redirect)) return redirect!;

        await _adminService.RejectLotAsync(id);
        TempData["SuccessMessage"] = "Đã từ chối bãi xe";
        return RedirectToAction("PendingLots");
    }

    [HttpGet]
    public async Task<IActionResult> Users(string? role)
    {
        if (!RequireAdmin(out var redirect)) return redirect!;

        var result = await _adminService.GetUsersAsync(role);
        ViewBag.Role = role;
        ViewBag.SuccessMessage = TempData["SuccessMessage"];

        return View(result?.Data ?? new());
    }

    [HttpPost]
    public async Task<IActionResult> ToggleActive(int id)
    {
        if (!RequireAdmin(out var redirect)) return redirect!;

        var result = await _adminService.ToggleUserActiveAsync(id);
        TempData["SuccessMessage"] = result?.Message ?? "Đã cập nhật";
        return RedirectToAction("Users");
    }
}