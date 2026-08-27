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
    [Route("api/v1/owner/prices")]
    [Authorize(Roles = "Owner")]
    public class PricesController : ControllerBase
    {
        private readonly ParkingDbContext _db;
        public PricesController(ParkingDbContext db) => _db = db;

        private int GetOwnerId() => int.Parse(User.FindFirst("OwnerId")!.Value);

        [HttpGet]
        public async Task<IActionResult> GetPrices([FromQuery] int parkingLotId)
        {
            var ownerId = GetOwnerId();
            var lot = await _db.ParkingLots.FirstOrDefaultAsync(l => l.ParkingLotId == parkingLotId);
            if (lot == null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy bãi xe"));
            if (lot.OwnerId != ownerId) return Forbid();

            var prices = await _db.Prices
                .Include(p => p.VehicleType)
                .Where(p => p.ParkingLotId == parkingLotId)
                .Select(p => new PriceResponse(
                    p.PriceId, p.ParkingLotId, p.VehicleTypeId, p.VehicleType.TypeName,
                    p.PriceType, p.UnitPrice))
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(prices));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrice(CreatePriceRequest req)
        {
            var ownerId = GetOwnerId();
            var lot = await _db.ParkingLots.FirstOrDefaultAsync(l => l.ParkingLotId == req.ParkingLotId);
            if (lot == null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy bãi xe"));
            if (lot.OwnerId != ownerId) return Forbid();

            var price = new Price
            {
                ParkingLotId = req.ParkingLotId,
                VehicleTypeId = req.VehicleTypeId,
                PriceType = req.PriceType,
                UnitPrice = req.UnitPrice
            };
            _db.Prices.Add(price);
            await _db.SaveChangesAsync();

            return Created("", ApiResponse<object>.Ok(new { price.PriceId }, "Thêm giá thành công"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrice(int id, UpdatePriceRequest req)
        {
            var ownerId = GetOwnerId();
            var price = await _db.Prices.Include(p => p.ParkingLot).FirstOrDefaultAsync(p => p.PriceId == id);
            if (price == null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy giá"));
            if (price.ParkingLot.OwnerId != ownerId) return Forbid();

            price.UnitPrice = req.UnitPrice;
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object?>.Ok(null, "Cập nhật giá thành công"));
        }
    }
}