using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagement.API.Data;
using ParkingManagement.API.DTOs;
using ParkingManagement.API.Helpers;
using ParkingManagement.API.Models;

namespace ParkingManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class BookingsController : ControllerBase
    {
        private readonly ParkingDbContext _db;

        public BookingsController(ParkingDbContext db)
        {
            _db = db;
        }

        // ================== KIỂM TRA CHỖ TRỐNG (UC-01 bước 2) ==================

        [HttpGet("parkinglots/{lotId}/availability")]
        public async Task<IActionResult> CheckAvailability(
     int lotId, [FromQuery] int vehicleTypeId,
     [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            if (endTime <= startTime)
                return BadRequest(ApiResponse<object>.Fail("Giờ kết thúc phải sau giờ bắt đầu"));

            var (total, available) = await GetAvailability(lotId, vehicleTypeId, startTime, endTime);

            // THÊM MỚI: tính luôn ước tính phí, dùng cùng logic giá như lúc tạo Booking thật
            decimal? estimatedAmount = null;
            var price = await _db.Prices.FirstOrDefaultAsync(p =>
                p.ParkingLotId == lotId && p.VehicleTypeId == vehicleTypeId && p.PriceType == "Booking");

            if (price != null)
            {
                var hours = (decimal)(endTime - startTime).TotalHours;
                estimatedAmount = Math.Ceiling(hours) * price.UnitPrice;
            }

            return Ok(ApiResponse<AvailabilityResponse>.Ok(
                new AvailabilityResponse(total, available, estimatedAmount)));
        }

        // Hàm dùng chung, tính TỔNG slot vật lý trừ đi số Booking đã xác nhận
        // bị TRÙNG khung giờ yêu cầu (không đếm slot cụ thể vì Booking không gán slot)
        private async Task<(int total, int available)> GetAvailability(
            int lotId, int vehicleTypeId, DateTime startTime, DateTime endTime)
        {
            var totalSlots = await _db.ParkingSlots
                .Where(s => s.ParkingArea.ParkingLotId == lotId && s.VehicleTypeId == vehicleTypeId)
                .CountAsync();

            var overlappingBookings = await _db.Bookings
                .Where(b => b.ParkingLotId == lotId
                    && b.VehicleTypeId == vehicleTypeId
                    && b.Status == "DaXacNhan"
                    && b.StartTime < endTime && b.EndTime > startTime) // điều kiện trùng khung giờ
                .CountAsync();

            return (totalSlots, totalSlots - overlappingBookings);
        }

        // ================== TẠO BOOKING (UC-01 bước 1-3) ==================

        [Authorize(Roles = "Customer")]
        [HttpPost("bookings")]
        public async Task<IActionResult> CreateBooking(CreateBookingRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.LicensePlate))
            {
                return BadRequest(
                    ApiResponse<object>.Fail("Vui lòng nhập biển số xe"));
            }

            if (req.EndTime <= req.StartTime)
                return BadRequest(ApiResponse<object>.Fail("Giờ kết thúc phải sau giờ bắt đầu"));

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            // QUAN TRỌNG: Transaction mức Serializable để tránh 2 khách cùng
            // đặt trùng chỗ trong lúc đang kiểm tra (race condition)
            using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var (total, available) = await GetAvailability(
                    req.ParkingLotId, req.VehicleTypeId, req.StartTime, req.EndTime);

                if (available <= 0)
                {
                    await transaction.RollbackAsync();
                    return Conflict(ApiResponse<object>.Fail(
                        "Bãi xe đã hết chỗ trống cho loại xe này trong khung giờ đã chọn"));
                }

                // Lấy đơn giá Booking (theo giờ) đang áp dụng
                var price = await _db.Prices.FirstOrDefaultAsync(p =>
                    p.ParkingLotId == req.ParkingLotId &&
                    p.VehicleTypeId == req.VehicleTypeId &&
                    p.PriceType == "Booking");

                if (price == null)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(ApiResponse<object>.Fail("Bãi xe chưa cấu hình giá cho loại xe này"));
                }

                var hours = (decimal)(req.EndTime - req.StartTime).TotalHours;
                var totalAmount = Math.Ceiling(hours) * price.UnitPrice; // làm tròn lên theo giờ

                var booking = new Booking
                {
                    UserId = userId,
                    ParkingLotId = req.ParkingLotId,
                    VehicleTypeId = req.VehicleTypeId,
                    LicensePlate = req.LicensePlate.Trim().ToUpper(),
                    StartTime = req.StartTime,
                    EndTime = req.EndTime,
                    Status = "ChoThanhToan",
                    IsPrepaid = false,
                    TotalAmount = totalAmount,
                    CreatedAt = DateTime.UtcNow,
                   
                };

                _db.Bookings.Add(booking);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Created("", ApiResponse<object>.Ok(
                    new { booking.BookingId, booking.TotalAmount },
                    "Tạo booking thành công, vui lòng thanh toán"));
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ================== THANH TOÁN (UC-01 bước 4-6) ==================

        [Authorize(Roles = "Customer")]
        [HttpPost("bookings/{id}/pay")]
        public async Task<IActionResult> PayBooking(int id, PayBookingRequest req)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound(ApiResponse<object>.Fail("Không tìm thấy booking"));

            if (booking.UserId != userId)
                return Forbid();

            if (booking.Status != "ChoThanhToan")
                return BadRequest(ApiResponse<object>.Fail("Booking không ở trạng thái chờ thanh toán"));

            // TODO: gọi cổng thanh toán thật ở đây. Hiện tại mô phỏng luôn thành công.
            var payment = new Payment
            {
                BookingId = booking.BookingId,
                Amount = booking.TotalAmount,
                PaymentType = "DatCho",
                PaymentMethod = req.PaymentMethod,
                Status = "ThanhCong",
                PaidAt = DateTime.UtcNow
            };
            _db.Payments.Add(payment);

            booking.Status = "DaXacNhan";
            booking.IsPrepaid = true;

            await _db.SaveChangesAsync();

            // TODO Module 6/UC-05: tạo Notification xác nhận đặt chỗ ở đây

            return Ok(ApiResponse<object>.Ok(
                new { booking.BookingId, booking.Status },
                "Thanh toán thành công, đặt chỗ đã được xác nhận"));
        }

        // ================== HỦY BOOKING ==================

        [Authorize(Roles = "Customer")]
        [HttpDelete("bookings/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound(ApiResponse<object>.Fail("Không tìm thấy booking"));

            if (booking.UserId != userId)
                return Forbid();

            if (booking.Status == "DaHoanTat")
                return BadRequest(ApiResponse<object>.Fail("Booking đã hoàn tất, không thể hủy"));

            booking.Status = "DaHuy";
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Đã hủy booking"));
        }

        // ================== LỊCH SỬ ĐẶT CHỖ ==================

        [Authorize(Roles = "Customer")]
        [HttpGet("bookings/my")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            var bookings = await _db.Bookings
                .Include(b => b.ParkingLot)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BookingResponse(
                    b.BookingId, b.ParkingLotId, b.ParkingLot.Name,
                    b.StartTime, b.EndTime, b.Status, b.IsPrepaid, b.TotalAmount))
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(bookings));
        }
        [Authorize(Roles = "Customer")]
        [HttpPost("bookings/{id}/extend")]
        public async Task<IActionResult> ExtendBooking(int id, ExtendBookingRequest req)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.BookingId == id);

                if (booking == null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy booking"));
                if (booking.UserId != userId) return Forbid();
                if (booking.Status != "DaXacNhan")
                    return BadRequest(ApiResponse<object>.Fail("Chỉ có thể gia hạn Booking đã xác nhận và đang gửi xe"));

                var hasActiveSession = await _db.ParkingSessions
                    .AnyAsync(s => s.BookingId == booking.BookingId && s.Status == "DangGui");
                if (!hasActiveSession)
                    return BadRequest(ApiResponse<object>.Fail("Chỉ gia hạn được khi xe đang thực sự gửi tại bãi"));

                var newEndTime = booking.EndTime.AddHours((double)req.ExtendedHours);

                // Kiểm tra xung đột: chỗ này có khách khác đặt kế tiếp trong khung giờ gia hạn không
                var hasConflict = await _db.Bookings.AnyAsync(b =>
                    b.BookingId != booking.BookingId &&
                    b.ParkingLotId == booking.ParkingLotId &&
                    b.VehicleTypeId == booking.VehicleTypeId &&
                    b.Status == "DaXacNhan" &&
                    b.StartTime < newEndTime && b.EndTime > booking.EndTime);

                if (hasConflict)
                {
                    await transaction.RollbackAsync();
                    return Conflict(ApiResponse<object>.Fail(
                        "Chỗ đỗ đã có khách khác đặt kế tiếp, không thể gia hạn. Vui lòng check-out đúng giờ."));
                }

                var price = await _db.Prices.FirstOrDefaultAsync(p =>
                    p.ParkingLotId == booking.ParkingLotId &&
                    p.VehicleTypeId == booking.VehicleTypeId &&
                    p.PriceType == "Booking");

                var additionalAmount = Math.Ceiling(req.ExtendedHours) * (price?.UnitPrice ?? 0);

                booking.EndTime = newEndTime;
                booking.TotalAmount += additionalAmount;

                var payment = new Payment
                {
                    BookingId = booking.BookingId,
                    Amount = additionalAmount,
                    PaymentType = "GiaHan",
                    PaymentMethod = "ViDienTu",
                    Status = "ThanhCong",
                    PaidAt = DateTime.UtcNow
                };
                _db.Payments.Add(payment);

                var extension = new BookingExtension
                {
                    BookingId = booking.BookingId,
                    ExtendedHours = req.ExtendedHours,
                    Amount = additionalAmount,
                    ExtendedAt = DateTime.UtcNow
                };
                _db.BookingExtensions.Add(extension);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(ApiResponse<ExtendBookingResponse>.Ok(
                    new ExtendBookingResponse(newEndTime, additionalAmount),
                    "Gia hạn thành công"));
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}