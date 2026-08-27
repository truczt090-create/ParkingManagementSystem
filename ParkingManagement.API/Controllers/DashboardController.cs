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
    public class DashboardController : ControllerBase
    {
        private readonly ParkingDbContext _db;

        public DashboardController(ParkingDbContext db)
        {
            _db = db;
        }

        // ================== OWNER: DOANH THU ==================

        [Authorize(Roles = "Owner")]
        [HttpGet("owner/dashboard/revenue")]
        public async Task<IActionResult> GetRevenue([FromQuery] string range = "day")
        {
            var ownerId = int.Parse(User.FindFirst("OwnerId")!.Value);

            // Lấy toàn bộ Payment thuộc các bãi xe của Owner này
            // (join qua Booking hoặc ParkingSession -> ParkingSlot -> ParkingArea -> ParkingLot)
            var paymentsQuery =
                from p in _db.Payments
                join b in _db.Bookings on p.BookingId equals b.BookingId into bj
                from b in bj.DefaultIfEmpty()
                join s in _db.ParkingSessions on p.ParkingSessionId equals s.ParkingSessionId into sj
                from s in sj.DefaultIfEmpty()
                where p.Status == "ThanhCong"
                select new { Payment = p, LotIdFromBooking = b != null ? b.ParkingLotId : (int?)null, Session = s };

            // Lọc theo bãi xe thuộc Owner — cần thêm điều kiện lot.OwnerId == ownerId,
            // ở đây minh họa đơn giản qua Booking; với walk-in cần join thêm ParkingSlot->ParkingArea->ParkingLot
            var myLotIds = await _db.ParkingLots
                .Where(l => l.OwnerId == ownerId)
                .Select(l => l.ParkingLotId)
                .ToListAsync();

            var payments = await paymentsQuery
                .Where(x => x.LotIdFromBooking != null && myLotIds.Contains(x.LotIdFromBooking.Value))
                .Select(x => new { x.Payment.Amount, x.Payment.PaidAt })
                .ToListAsync();

            var total = payments.Sum(p => p.Amount);

            var breakdown = range switch
            {
                "month" => payments.GroupBy(p => p.PaidAt.ToString("yyyy-MM"))
                    .Select(g => new RevenuePoint(g.Key, g.Sum(x => x.Amount))).ToList(),
                "year" => payments.GroupBy(p => p.PaidAt.ToString("yyyy"))
                    .Select(g => new RevenuePoint(g.Key, g.Sum(x => x.Amount))).ToList(),
                _ => payments.GroupBy(p => p.PaidAt.ToString("yyyy-MM-dd"))
                    .Select(g => new RevenuePoint(g.Key, g.Sum(x => x.Amount))).ToList(),
            };

            return Ok(ApiResponse<RevenueResponse>.Ok(new RevenueResponse(total, breakdown)));
        }

        // ================== OWNER: TỶ LỆ LẤP ĐẦY ==================

        [Authorize(Roles = "Owner")]
        [HttpGet("owner/dashboard/occupancy")]
        public async Task<IActionResult> GetOccupancy()
        {
            var ownerId = int.Parse(User.FindFirst("OwnerId")!.Value);

            var totalSlots = await _db.ParkingSlots
                .Where(s => s.ParkingArea.ParkingLot.OwnerId == ownerId)
                .CountAsync();

            var occupiedSlots = await _db.ParkingSlots
                .Where(s => s.ParkingArea.ParkingLot.OwnerId == ownerId && s.Status == "Đang sử dụng")
                .CountAsync();

            var rate = totalSlots == 0 ? 0 : Math.Round((double)occupiedSlots / totalSlots * 100, 1);

            return Ok(ApiResponse<OccupancyResponse>.Ok(
                new OccupancyResponse(totalSlots, occupiedSlots, rate)));
        }
        // ================== OWNER: TỔNG QUAN NHANH ==================

        [Authorize(Roles = "Owner")]
        [HttpGet("owner/dashboard/summary")]
        public async Task<IActionResult> GetSummary()
        {
            var ownerId = int.Parse(
                User.FindFirst("OwnerId")!.Value);

            // Các bãi xe thuộc Owner đang đăng nhập
            var myLotIds = await _db.ParkingLots
                .Where(l => l.OwnerId == ownerId)
                .Select(l => l.ParkingLotId)
                .ToListAsync();

            // 1. Số bãi xe
            var parkingLotCount = myLotIds.Count;

            // 2. Số khu vực
            var areaCount = await _db.ParkingAreas
                .CountAsync(a =>
                    myLotIds.Contains(a.ParkingLotId));

            // 3. Tổng số chỗ xe
            var totalSlots = await _db.ParkingSlots
                .CountAsync(s =>
                    myLotIds.Contains(
                        s.ParkingArea.ParkingLotId));

            // 4. Số nhân viên
            var employeeCount = await _db.Employees
                .CountAsync(e =>
                    myLotIds.Contains(e.ParkingLotId));

            // 5. Đánh giá trung bình
            var reviewQuery = _db.Reviews
                .Where(r =>
                    myLotIds.Contains(r.ParkingLotId));

            var averageRating = await reviewQuery.AnyAsync()
                ? Math.Round(
                    await reviewQuery.AverageAsync(
                        r => (double)r.Rating),
                    1)
                : 0;

            // 6. Tổng số lượt đặt chỗ
            var bookingCount = await _db.Bookings
                .CountAsync(b =>
                    myLotIds.Contains(b.ParkingLotId));

            return Ok(ApiResponse<object>.Ok(new
            {
                parkingLotCount,
                areaCount,
                totalSlots,
                employeeCount,
                averageRating,
                bookingCount
            }));
        }
        // ================== ADMIN: DASHBOARD TỔNG ==================

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/dashboard")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var totalUsers = await _db.Users.CountAsync();
            var totalLots = await _db.ParkingLots.CountAsync();
            var pendingLots = await _db.ParkingLots.CountAsync(l => l.Status == "Pending");
            var totalRevenue = await _db.Payments
                .Where(p => p.Status == "ThanhCong")
                .SumAsync(p => p.Amount);

            return Ok(ApiResponse<AdminDashboardResponse>.Ok(
                new AdminDashboardResponse(totalUsers, totalLots, pendingLots, totalRevenue)));
        }
    }
}