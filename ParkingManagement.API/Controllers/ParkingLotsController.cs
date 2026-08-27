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
    public class ParkingLotsController : ControllerBase
    {
        private readonly ParkingDbContext _db;

        public ParkingLotsController(ParkingDbContext db)
        {
            _db = db;
        }

        // ================== PUBLIC: KHÁCH TÌM BÃI XE ==================

        [HttpGet("parkinglots")]
        public async Task<IActionResult> GetApprovedLots([FromQuery] string? keyword)
        {
            var query = _db.ParkingLots.Where(p => p.Status == "Approved");

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.Name.Contains(keyword) || p.Address.Contains(keyword));

            var result = await query.Select(p => new ParkingLotResponse(
                p.ParkingLotId, p.Name, p.Address, p.Latitude, p.Longitude,
                p.OpenTime, p.CloseTime, p.Status)).ToListAsync();

            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpGet("parkinglots/{id}")]
        public async Task<IActionResult> GetLotDetail(int id)
        {
            var lot = await _db.ParkingLots.FirstOrDefaultAsync(p => p.ParkingLotId == id && p.Status == "Approved");

            if (lot == null)
                return NotFound(ApiResponse<object>.Fail("Không tìm thấy bãi xe"));

            return Ok(ApiResponse<object>.Ok(new ParkingLotResponse(
                lot.ParkingLotId, lot.Name, lot.Address, lot.Latitude, lot.Longitude,
                lot.OpenTime, lot.CloseTime, lot.Status)));
        }

        // ================== OWNER: QUẢN LÝ BÃI CỦA MÌNH ==================

        [Authorize(Roles = "Owner")]
        [HttpPost("owner/parkinglots")]
        public async Task<IActionResult> CreateLot(CreateParkingLotRequest req)
        {
            var ownerId = int.Parse(User.FindFirst("OwnerId")!.Value);

            var lot = new ParkingLot
            {
                OwnerId = ownerId,
                Name = req.Name,
                Address = req.Address,
                Latitude = req.Latitude,
                Longitude = req.Longitude,
                OpenTime = req.OpenTime,
                CloseTime = req.CloseTime,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _db.ParkingLots.Add(lot);
            await _db.SaveChangesAsync();

            return Created("", ApiResponse<object>.Ok(
                new { lot.ParkingLotId },
                "Đăng ký bãi xe thành công, đang chờ Admin duyệt"));
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("owner/parkinglots/my")]
        public async Task<IActionResult> GetMyLots()
        {
            var ownerId = int.Parse(User.FindFirst("OwnerId")!.Value);

            var lots = await _db.ParkingLots.Where(p => p.OwnerId == ownerId)
                .Select(p => new ParkingLotResponse(
                    p.ParkingLotId, p.Name, p.Address, p.Latitude, p.Longitude,
                    p.OpenTime, p.CloseTime, p.Status))
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(lots));
        }
        [Authorize(Roles = "Owner")]
        [HttpGet("owner/parkinglots/{id}")]
        public async Task<IActionResult> GetOwnerLot(int id)
        {
            var ownerId = int.Parse(
                User.FindFirst("OwnerId")!.Value);

            var lot = await _db.ParkingLots
                .FirstOrDefaultAsync(p =>
                    p.ParkingLotId == id &&
                    p.OwnerId == ownerId);

            if (lot == null)
            {
                return NotFound(
                    ApiResponse<object>.Fail(
                        "Không tìm thấy bãi xe"));
            }

            var result = new ParkingLotResponse(
                lot.ParkingLotId,
                lot.Name,
                lot.Address,
                lot.Latitude,
                lot.Longitude,
                lot.OpenTime,
                lot.CloseTime,
                lot.Status
            );

            return Ok(
                ApiResponse<object>.Ok(result));
        }
        [Authorize(Roles = "Owner")]
        [HttpPut("owner/parkinglots/{id}")]
        public async Task<IActionResult> UpdateLot(int id, UpdateParkingLotRequest req)
        {
            var ownerId = int.Parse(User.FindFirst("OwnerId")!.Value);
            var lot = await _db.ParkingLots.FirstOrDefaultAsync(p => p.ParkingLotId == id);

            if (lot == null)
                return NotFound(ApiResponse<object>.Fail("Không tìm thấy bãi xe"));

            // QUAN TRỌNG: chặn Owner A sửa bãi của Owner B
            if (lot.OwnerId != ownerId)
                return Forbid();

            lot.Name = req.Name;
            lot.Address = req.Address;
            lot.Latitude = req.Latitude;
            lot.Longitude = req.Longitude;
            lot.OpenTime = req.OpenTime;
            lot.CloseTime = req.CloseTime;

            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Cập nhật thành công"));
        }
        [Authorize(Roles = "Owner")]
        [HttpPost("owner/parkinglots/{id}/images")]
        public async Task<IActionResult> AddParkingLotImage(
    int id,
    CreateParkingLotImageRequest req)
        {
            var ownerId = int.Parse(
                User.FindFirst("OwnerId")!.Value);

            var lot = await _db.ParkingLots
                .FirstOrDefaultAsync(p =>
                    p.ParkingLotId == id &&
                    p.OwnerId == ownerId);

            if (lot == null)
            {
                return NotFound(
                    ApiResponse<object>.Fail(
                        "Không tìm thấy bãi xe"));
            }

            if (string.IsNullOrWhiteSpace(req.ImageUrl))
            {
                return BadRequest(
                    ApiResponse<object>.Fail(
                        "Đường dẫn ảnh không hợp lệ"));
            }

            // Nếu ảnh mới được chọn làm ảnh chính
            // thì bỏ trạng thái ảnh chính của những ảnh cũ
            if (req.IsPrimary)
            {
                var oldPrimaryImages = await _db.Images
                    .Where(i =>
                        i.ParkingLotId == id &&
                        i.IsPrimary)
                    .ToListAsync();

                foreach (var image in oldPrimaryImages)
                {
                    image.IsPrimary = false;
                }
            }

            // Nếu bãi chưa có ảnh nào thì ảnh đầu tiên
            // tự động trở thành ảnh chính
            var hasImage = await _db.Images
                .AnyAsync(i => i.ParkingLotId == id);

            var newImage = new Image
            {
                ParkingLotId = id,
                ImageUrl = req.ImageUrl,
                IsPrimary = !hasImage || req.IsPrimary
            };

            _db.Images.Add(newImage);

            await _db.SaveChangesAsync();

            return Created(
                "",
                ApiResponse<object>.Ok(
                    new
                    {
                        newImage.ImageId,
                        newImage.ImageUrl,
                        newImage.IsPrimary
                    },
                    "Thêm ảnh bãi xe thành công"));
        }
        [HttpGet("vehicletypes")]
        public async Task<IActionResult> GetVehicleTypes()
        {
            var types = await _db.VehicleTypes
                .Select(v => new { v.VehicleTypeId, v.TypeName })
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(types));
        }
        [HttpGet("parkinglots/{id}/stats")]
        public async Task<IActionResult> GetQuickStats(int id)
        {
            var lot = await _db.ParkingLots.FirstOrDefaultAsync(l => l.ParkingLotId == id);
            if (lot == null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy bãi xe"));

            var totalSlots = await _db.ParkingSlots.Where(s => s.ParkingArea.ParkingLotId == id).CountAsync();
            var availableSlots = await _db.ParkingSlots.Where(s => s.ParkingArea.ParkingLotId == id && s.Status == "Trống").CountAsync();

            var prices = await _db.Prices
                .Where(p => p.ParkingLotId == id && p.PriceType == "Booking")
                .Select(p => p.UnitPrice)
                .ToListAsync();

            return Ok(ApiResponse<QuickStatsResponse>.Ok(new QuickStatsResponse(
                totalSlots, availableSlots,
                prices.Any() ? prices.Min() : null,
                prices.Any() ? prices.Max() : null,
                DateTime.Now)));
        }
       
        [HttpGet("parkinglots/{id}/images")]
        public async Task<IActionResult> GetParkingLotImages(int id)
        {
            var images = await _db.Images
                .Where(i => i.ParkingLotId == id)
                .OrderByDescending(i => i.IsPrimary)
                .ThenBy(i => i.ImageId)
                .Select(i => new
                {
                    i.ImageId,
                    i.ImageUrl,
                    i.IsPrimary
                })
                .ToListAsync();

            return Ok(
                ApiResponse<object>.Ok(images));
        }
        [HttpGet("parkinglots/{id}/slots")]
        public async Task<IActionResult> GetParkingSlots(int id)
        {
            var lotExists = await _db.ParkingLots
                .AnyAsync(p => p.ParkingLotId == id);

            if (!lotExists)
            {
                return NotFound(
                    ApiResponse<object>.Fail("Không tìm thấy bãi xe"));
            }

            var slots = await _db.ParkingSlots
                .Where(s => s.ParkingArea.ParkingLotId == id)
                .OrderBy(s => s.SlotCode)
                .Select(s => new
                {
                    s.ParkingSlotId,
                    s.SlotCode,
                    s.Status
                })
                .ToListAsync();

            return Ok(slots);
        }
    }
}