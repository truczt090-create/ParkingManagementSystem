using System.Data;
using System.Security.Claims;
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
    [Authorize(Roles = "Employee")]
    public class ParkingSessionsController : ControllerBase
    {
        private readonly ParkingDbContext _db;

        public ParkingSessionsController(ParkingDbContext db)
        {
            _db = db;
        }

        private int GetMyLotId() => int.Parse(User.FindFirst("ParkingLotId")!.Value);
        private async Task<int?> GetMyEmployeeIdAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) ||
                !int.TryParse(userIdClaim, out var userId))
            {
                return null;
            }

            var employee = await _db.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId);

            return employee?.EmployeeId;
        }

        // ================== DANH SÁCH SLOT TRỐNG (UC-02 bước 4) ==================

        [HttpGet("employee/slots")]
        public async Task<IActionResult> GetAvailableSlots()
        {
            var lotId = GetMyLotId();

            var slots = await _db.ParkingSlots
                .Where(s => s.ParkingArea.ParkingLotId == lotId && s.Status == "Trống")
                .Select(s => new AvailableSlotResponse(s.ParkingSlotId, s.SlotCode))
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(slots));
        }
        // ================== DASHBOARD QUẢN LÝ XE ==================

        [HttpGet("employee/vehicle-dashboard")]
        public async Task<IActionResult> GetVehicleDashboard()
        {
            var lotId = GetMyLotId();

            var lot = await _db.ParkingLots
                .FirstOrDefaultAsync(p => p.ParkingLotId == lotId);

            if (lot == null)
            {
                return NotFound(
                    ApiResponse<object>.Fail("Không tìm thấy bãi xe của nhân viên"));
            }

            var slots = await _db.ParkingSlots
                .Where(s => s.ParkingArea.ParkingLotId == lotId)
                .OrderBy(s => s.SlotCode)
                .Select(s => new
                {
                    s.ParkingSlotId,
                    s.SlotCode,
                    s.Status,

                    ActiveSession = _db.ParkingSessions
                        .Where(ps =>
                            ps.ParkingSlotId == s.ParkingSlotId &&
                            ps.Status == "DangGui")
                        .Select(ps => new
                        {
                            ps.ParkingSessionId,
                            ps.LicensePlate,
                            ps.BookingId,
                            ps.CheckInTime
                        })
                        .FirstOrDefault()
                })
                .ToListAsync();

            var result = new
            {
                ParkingLotName = lot.Name,

                TotalSlots = slots.Count,

                AvailableSlots =
                    slots.Count(s => s.Status == "Trống"),

                OccupiedSlots =
                    slots.Count(s => s.Status == "Đang sử dụng"),

                ReservedSlots = 0,

                Slots = slots.Select(s => new
                {
                    s.ParkingSlotId,
                    s.SlotCode,
                    s.Status,

                    ParkingSessionId =
                        s.ActiveSession != null
                            ? (int?)s.ActiveSession.ParkingSessionId
                            : null,

                    LicensePlate =
                        s.ActiveSession != null
                            ? s.ActiveSession.LicensePlate
                            : null,

                    BookingId =
                        s.ActiveSession != null
                            ? s.ActiveSession.BookingId
                            : null,

                    CheckInTime =
                        s.ActiveSession != null
                            ? (DateTime?)s.ActiveSession.CheckInTime
                            : null
                }).ToList()
            };

            return Ok(
                ApiResponse<object>.Ok(result));
        }

        // ================== CHECK-IN: CÓ BOOKING (UC-02 nhánh "Có") ==================

        [HttpPost("checkin/with-booking")]
        public async Task<IActionResult> CheckInWithBooking(CheckInWithBookingRequest req)
        {
            var lotId = GetMyLotId();
            var employeeId = await GetMyEmployeeIdAsync();

            if (employeeId == null)
            {
                return BadRequest(
                    ApiResponse<object>.Fail(
                        "Tài khoản này chưa được liên kết với hồ sơ nhân viên."
                    )
                );
            }
            using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.BookingId == req.BookingId);

                if (booking == null || booking.ParkingLotId != lotId)
                    return NotFound(ApiResponse<object>.Fail("Không tìm thấy booking tại bãi này"));

                if (booking.Status != "DaXacNhan")
                    return BadRequest(ApiResponse<object>.Fail("Booking chưa thanh toán hoặc đã bị hủy/hoàn tất"));

                var alreadyCheckedIn = await _db.ParkingSessions
                    .AnyAsync(s => s.BookingId == booking.BookingId && s.Status == "DangGui");
                if (alreadyCheckedIn)
                    return BadRequest(ApiResponse<object>.Fail("Booking này đã check-in trước đó"));

                var slot = await _db.ParkingSlots.FirstOrDefaultAsync(s =>
                    s.ParkingSlotId == req.ParkingSlotId &&
                    s.Status == "Trống" &&
                    s.ParkingArea.ParkingLotId == lotId);

                if (slot == null)
                {
                    await transaction.RollbackAsync();
                    return Conflict(ApiResponse<object>.Fail("Vị trí đỗ này vừa có xe khác chiếm, vui lòng chọn vị trí khác"));
                }

                if (slot.VehicleTypeId != booking.VehicleTypeId)
                    return BadRequest(ApiResponse<object>.Fail("Vị trí đỗ không đúng loại xe của booking"));

                slot.Status = "Đang sử dụng";

                var session = new ParkingSession
                {
                    BookingId = booking.BookingId,
                    ParkingSlotId = slot.ParkingSlotId,
                    LicensePlate = req.LicensePlate,
                    VehicleTypeId = booking.VehicleTypeId,
                    CheckInTime = DateTime.UtcNow,
                    EmployeeIdCheckIn = employeeId.Value,
                    Status = "DangGui",
                    CreatedAt = DateTime.UtcNow
                };

                _db.ParkingSessions.Add(session);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Created("", ApiResponse<object>.Ok(
                    new CheckInResponse(session.ParkingSessionId, slot.SlotCode, session.CheckInTime),
                    "Check-in thành công"));
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ================== CHECK-IN: KHÁCH VÃNG LAI (UC-02 nhánh "Không") ==================

        [HttpPost("checkin/walkin")]
        public async Task<IActionResult> CheckInWalkin(CheckInWalkinRequest req)
        {
            var lotId = GetMyLotId();
            var employeeId = await GetMyEmployeeIdAsync();

            if (employeeId == null)
            {
                return BadRequest(
                    ApiResponse<object>.Fail(
                        "Tài khoản này chưa được liên kết với hồ sơ nhân viên."
                    )
                );
            }
            using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var slot = await _db.ParkingSlots.FirstOrDefaultAsync(s =>
                    s.ParkingSlotId == req.ParkingSlotId &&
                    s.Status == "Trống" &&
                    s.ParkingArea.ParkingLotId == lotId);

                if (slot == null)
                {
                    await transaction.RollbackAsync();
                    return Conflict(ApiResponse<object>.Fail("Vị trí đỗ không hợp lệ hoặc đã có xe khác chiếm"));
                }

                if (slot.VehicleTypeId != req.VehicleTypeId)
                    return BadRequest(ApiResponse<object>.Fail("Vị trí đỗ không đúng loại xe"));

                slot.Status = "Đang sử dụng";

                var session = new ParkingSession
                {
                    BookingId = null,   // khách vãng lai
                    ParkingSlotId = slot.ParkingSlotId,
                    LicensePlate = req.LicensePlate,
                    VehicleTypeId = req.VehicleTypeId,
                    CheckInTime = DateTime.UtcNow,
                    EmployeeIdCheckIn = employeeId.Value,
                    Status = "DangGui",
                    CreatedAt = DateTime.UtcNow
                };

                _db.ParkingSessions.Add(session);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Created("", ApiResponse<object>.Ok(
                    new CheckInResponse(session.ParkingSessionId, slot.SlotCode, session.CheckInTime),
                    "Check-in khách vãng lai thành công"));
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ================== CHECK-OUT (UC-03: 3 nhánh tính phí) ==================

        [HttpPost("checkout/{sessionId}")]
        public async Task<IActionResult> CheckOut(int sessionId)
        {
            var lotId = GetMyLotId();
            var employeeId = await GetMyEmployeeIdAsync();

            if (employeeId == null)
            {
                return BadRequest(
                    ApiResponse<object>.Fail(
                        "Tài khoản này chưa được liên kết với hồ sơ nhân viên."
                    )
                );
            }

            using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var session = await _db.ParkingSessions
     .Include(s => s.Booking)
     .Include(s => s.ParkingSlot)
         .ThenInclude(s => s.ParkingArea)
     .FirstOrDefaultAsync(s => s.ParkingSessionId == sessionId);
                if (session == null ||
                    session.ParkingSlot == null ||
                    session.ParkingSlot.ParkingArea == null ||
                    session.ParkingSlot.ParkingArea.ParkingLotId != lotId)
                {
                    return NotFound(
                        ApiResponse<object>.Fail(
                            "Không tìm thấy phiên gửi xe tại bãi này"
                        )
                    );
                }

                if (session.Status != "DangGui")
                    return BadRequest(ApiResponse<object>.Fail("Phiên gửi xe đã checkout trước đó"));

                var checkOutTime = DateTime.UtcNow;
                decimal amount = 0;
                string customerType;
                bool isOvertime = false;
                decimal? overtimeHours = null;

                if (session.BookingId == null)
                {
                    // NHÁNH 1: Khách vãng lai — tính theo giờ thực tế
                    customerType = "VangLai";
                    var hours = (decimal)Math.Ceiling((checkOutTime - session.CheckInTime).TotalHours);
                    var price = await _db.Prices.FirstOrDefaultAsync(p =>
                        p.ParkingLotId == lotId && p.VehicleTypeId == session.VehicleTypeId && p.PriceType == "WalkIn");

                    amount = hours * (price?.UnitPrice ?? 0);
                }
                else
                {
                    customerType = "Booking";
                    var booking = session.Booking!;

                    if (checkOutTime <= booking.EndTime)
                    {
                        // NHÁNH 2: Đúng giờ — miễn phí
                        amount = 0;
                    }
                    else
                    {
                        // NHÁNH 3: Quá giờ — tính phụ phí overtime
                        isOvertime = true;
                        var extraHours = (decimal)Math.Ceiling((checkOutTime - booking.EndTime).TotalHours);
                        overtimeHours = extraHours;

                        var price = await _db.Prices.FirstOrDefaultAsync(p =>
                            p.ParkingLotId == lotId && p.VehicleTypeId == session.VehicleTypeId && p.PriceType == "Overtime");

                        amount = extraHours * (price?.UnitPrice ?? 0);
                    }

                    booking.Status = "DaHoanTat";
                }

                // Giải phóng chỗ đỗ + cập nhật session
                session.ParkingSlot.Status = "Trống";
                session.Status = "DaCheckout";
                session.CheckOutTime = checkOutTime;
                session.EmployeeIdCheckOut = employeeId.Value;

                // Chỉ tạo Payment nếu có phát sinh phí — luôn là tiền mặt (theo đúng nghiệp vụ đã chốt)
                if (amount > 0)
                {
                    _db.Payments.Add(new Payment
                    {
                        BookingId = session.BookingId,
                        ParkingSessionId = session.ParkingSessionId,
                        Amount = amount,
                        PaymentType = customerType == "VangLai" ? "VangLai" : "Overtime",
                        PaymentMethod = "TienMat",
                        Status = "ThanhCong",
                        PaidAt = checkOutTime
                    });
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(ApiResponse<object>.Ok(
                    new CheckoutResponse(sessionId, customerType, isOvertime, overtimeHours, amount, "TienMat"),
                    amount > 0 ? "Vui lòng thu tiền mặt tại quầy" : "Checkout thành công, không phát sinh phí"));
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ================== DANH SÁCH XE ĐANG GỬI ==================

        [HttpGet("sessions/active")]
        public async Task<IActionResult> GetActiveSessions()
        {
            var lotId = GetMyLotId();

            var sessions = await _db.ParkingSessions
                .Where(s => s.Status == "DangGui" && s.ParkingSlot.ParkingArea.ParkingLotId == lotId)
                .Select(s => new { s.ParkingSessionId, s.LicensePlate, s.CheckInTime, SlotCode = s.ParkingSlot.SlotCode })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(sessions));
        }
    }
}