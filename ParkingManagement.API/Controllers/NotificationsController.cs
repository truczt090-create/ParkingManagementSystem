using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagement.API.Data;
using ParkingManagement.API.DTOs;
using ParkingManagement.API.Helpers;

namespace ParkingManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly ParkingDbContext _db;
        public NotificationsController(ParkingDbContext db) => _db = db;

        private int GetUserId() => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = GetUserId();

            var notifications = await _db.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(30)
                .Select(n => new NotificationResponse(
                    n.NotificationId, n.Title, n.Content ?? "", n.Type, n.IsRead, n.CreatedAt))
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(notifications));
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetUserId();
            var notif = await _db.Notifications.FirstOrDefaultAsync(n => n.NotificationId == id && n.UserId == userId);

            if (notif == null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy thông báo"));

            notif.IsRead = true;
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Đã đánh dấu đã đọc"));
        }
    }
}