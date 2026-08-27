using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagement.API.Data;
using ParkingManagement.API.DTOs;
using ParkingManagement.API.Helpers;

namespace ParkingManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ParkingDbContext _db;
        public AdminController(ParkingDbContext db) => _db = db;

        // DUYỆT BÃI XE 

        [HttpGet("admin/parkinglots")]
        public async Task<IActionResult> GetLotsByStatus([FromQuery] string status = "Pending")
        {
            var lots = await _db.ParkingLots
                .Include(l => l.Owner).ThenInclude(o => o.User)
                .Where(l => l.Status == status)
                .Select(l => new ParkingLotSummaryResponse(
                    l.ParkingLotId, l.Name, l.Address, l.Status, l.Owner.User.FullName))
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(lots));
        }

        [HttpPost("admin/parkinglots/{id}/approve")]
        public async Task<IActionResult> ApproveLot(int id)
        {
            var lot = await _db.ParkingLots.FindAsync(id);
            if (lot == null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy bãi xe"));

            lot.Status = "Approved";
            await _db.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok(null, "Đã duyệt bãi xe"));
        }

        [HttpPost("admin/parkinglots/{id}/reject")]
        public async Task<IActionResult> RejectLot(int id)
        {
            var lot = await _db.ParkingLots.FindAsync(id);
            if (lot == null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy bãi xe"));

            lot.Status = "Rejected";
            await _db.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok(null, "Đã từ chối bãi xe"));
        }

        //  QUẢN LÝ USER

        [HttpGet("admin/users")]
        public async Task<IActionResult> GetUsers([FromQuery] string? role)
        {
            var query = _db.Users.Include(u => u.Role).AsQueryable();

            if (!string.IsNullOrEmpty(role))
                query = query.Where(u => u.Role.RoleName == role);

            var users = await query
                .Select(u => new UserSummaryResponse(
                    u.UserId, u.FullName, u.Email, u.Phone ?? "", u.Role.RoleName, u.IsActive, u.CreatedAt))
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(users));
        }

        [HttpPost("admin/users/{id}/toggle-active")]
        public async Task<IActionResult> ToggleUserActive(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy user"));

            user.IsActive = !user.IsActive;
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(new { user.IsActive },
                user.IsActive ? "Đã mở khóa tài khoản" : "Đã khóa tài khoản"));
        }
    }
}