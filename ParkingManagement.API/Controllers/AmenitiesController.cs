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
    public class AmenitiesController : ControllerBase
    {
        private readonly ParkingDbContext _db;
        public AmenitiesController(ParkingDbContext db) => _db = db;

        [HttpGet("parkinglots/{lotId}/amenities")]
        public async Task<IActionResult> GetAmenities(int lotId)
        {
            var amenities = await _db.ParkingLotAmenities
                .Where(a => a.ParkingLotId == lotId)
                .Select(a => new AmenityResponse(a.AmenityId, a.Content))
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(amenities));
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("owner/parkinglots/{lotId}/amenities")]
        public async Task<IActionResult> AddAmenity(int lotId, CreateAmenityRequest req)
        {
            var ownerId = int.Parse(User.FindFirst("OwnerId")!.Value);
            var lot = await _db.ParkingLots.FirstOrDefaultAsync(l => l.ParkingLotId == lotId);

            if (lot == null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy bãi xe"));
            if (lot.OwnerId != ownerId) return Forbid();

            _db.ParkingLotAmenities.Add(new ParkingLotAmenity { ParkingLotId = lotId, Content = req.Content });
            await _db.SaveChangesAsync();

            return Created("", ApiResponse<object>.Ok(null, "Đã thêm tiện ích"));
        }
    }
}