using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Web.Services.Interfaces;

namespace ParkingManagement.Web.Controllers;

public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;
    public NotificationController(INotificationService notificationService) => _notificationService = notificationService;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            return RedirectToAction("Login", "Account");

        var result = await _notificationService.GetMyNotificationsAsync();
        return View(result?.Data ?? new());
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return RedirectToAction("Index");
    }
}