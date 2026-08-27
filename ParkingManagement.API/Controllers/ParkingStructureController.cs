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
    [Authorize(Roles = "Owner")]
    public class ParkingStructureController : ControllerBase
    {
        private readonly ParkingDbContext _db;
        public ParkingStructureController(ParkingDbContext db) => _db = db;

        private int GetOwnerId() => int.Parse(User.FindFirst("OwnerId")!.Value);

        [HttpPost("owner/parkinglots/{lotId}/areas")]
        public async Task<IActionResult> CreateArea(int lotId, CreateAreaRequest req)
        {
            var ownerId = GetOwnerId();
            var lot = await _db.ParkingLots.FirstOrDefaultAsync(l => l.ParkingLotId == lotId);
            if (lot == null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy bãi xe"));
            if (lot.OwnerId != ownerId) return Forbid();

            var area = new ParkingArea { ParkingLotId = lotId, Name = req.Name };
            _db.ParkingAreas.Add(area);
            await _db.SaveChangesAsync();

            return Created("", ApiResponse<object>.Ok(new { area.ParkingAreaId }, "Tạo khu vực thành công"));
        }

        [HttpGet("owner/parkinglots/{lotId}/areas")]
        public async Task<IActionResult> GetAreas(int lotId)
        {
            var ownerId = GetOwnerId();
            var lot = await _db.ParkingLots.FirstOrDefaultAsync(l => l.ParkingLotId == lotId);
            if (lot == null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy bãi xe"));
            if (lot.OwnerId != ownerId) return Forbid();

            var areas = await _db.ParkingAreas
                .Where(a => a.ParkingLotId == lotId)
                .Select(a => new AreaResponse(a.ParkingAreaId, a.Name))
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(areas));
        }

        [HttpPost("owner/areas/{areaId}/slots")]
        public async Task<IActionResult> CreateSlot(int areaId, CreateSlotRequest req)
        {
            var ownerId = GetOwnerId();
            var area = await _db.ParkingAreas.Include(a => a.ParkingLot)
                .FirstOrDefaultAsync(a => a.ParkingAreaId == areaId);
            if (area == null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy khu vực"));
            if (area.ParkingLot.OwnerId != ownerId) return Forbid();

            var slot = new ParkingSlot
            {
                ParkingAreaId = areaId,
                VehicleTypeId = req.VehicleTypeId,
                SlotCode = req.SlotCode,
                Status = "Trống"
            };
            _db.ParkingSlots.Add(slot);
            await _db.SaveChangesAsync();

            return Created("", ApiResponse<object>.Ok(new { slot.ParkingSlotId }, "Tạo vị trí đỗ thành công"));
        }
        [HttpPost("owner/areas/{areaId}/slots/bulk")]
        public async Task<IActionResult> CreateBulkSlots(
    int areaId,
    CreateBulkSlotsRequest req)
        {
            var ownerId = GetOwnerId();

            var area = await _db.ParkingAreas
                .Include(a => a.ParkingLot)
                .FirstOrDefaultAsync(a =>
                    a.ParkingAreaId == areaId);

            if (area == null)
                return NotFound(
                    ApiResponse<object>.Fail(
                        "Không tìm thấy khu vực"));

            if (area.ParkingLot.OwnerId != ownerId)
                return Forbid();

            if (req.Quantity <= 0)
                return BadRequest(
                    ApiResponse<object>.Fail(
                        "Số lượng chỗ phải lớn hơn 0"));

            if (req.Quantity > 500)
                return BadRequest(
                    ApiResponse<object>.Fail(
                        "Không thể tạo quá 500 chỗ một lần"));

            var prefix = string.IsNullOrWhiteSpace(req.Prefix)
                ? "S"
                : req.Prefix.Trim().ToUpper();

            var existingCodes = await _db.ParkingSlots
                .Where(s => s.ParkingAreaId == areaId)
                .Select(s => s.SlotCode)
                .ToListAsync();

            var slots = new List<ParkingSlot>();

            var number = 1;

            while (slots.Count < req.Quantity)
            {
                var code = $"{prefix}-{number:D2}";

                if (!existingCodes.Contains(code))
                {
                    slots.Add(new ParkingSlot
                    {
                        ParkingAreaId = areaId,
                        VehicleTypeId = req.VehicleTypeId,
                        SlotCode = code,
                        Status = "Trống"
                    });

                    existingCodes.Add(code);
                }

                number++;
            }

            _db.ParkingSlots.AddRange(slots);

            await _db.SaveChangesAsync();

            return Ok(
                ApiResponse<object>.Ok(
                    new
                    {
                        CreatedCount = slots.Count
                    },
                    $"Đã tạo {slots.Count} vị trí đỗ"));
        }
        [HttpGet("owner/areas/{areaId}/slots")]
        public async Task<IActionResult> GetSlots(int areaId)
        {
            var ownerId = GetOwnerId();
            var area = await _db.ParkingAreas.Include(a => a.ParkingLot)
                .FirstOrDefaultAsync(a => a.ParkingAreaId == areaId);
            if (area == null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy khu vực"));
            if (area.ParkingLot.OwnerId != ownerId) return Forbid();

            var slots = await _db.ParkingSlots
                .Where(s => s.ParkingAreaId == areaId)
                .Select(s => new SlotResponse(s.ParkingSlotId, s.SlotCode, s.Status, s.VehicleTypeId))
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(slots));
        }
    }
}