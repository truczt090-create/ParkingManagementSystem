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
    [Route("api/v1/parkinglots/{lotId}/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly ParkingDbContext _db;
        public ReviewsController(ParkingDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetReviews(int lotId, [FromQuery] int page = 1, [FromQuery] int pageSize = 6)
        {
            var allReviews = await _db.Reviews
                .Include(r => r.User)
                .Where(r => r.ParkingLotId == lotId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewItemResponse(
                    r.ReviewId, r.User.FullName, r.User.AvatarUrl, r.Rating, r.Comment, r.OwnerReply, r.CreatedAt))
                .ToListAsync();

            var avg = allReviews.Any() ? allReviews.Average(r => r.Rating) : 0;

            var breakdown = new Dictionary<int, int>();
            for (int star = 1; star <= 5; star++)
                breakdown[star] = allReviews.Count(r => r.Rating == star);

            var totalPages = allReviews.Any() ? (int)Math.Ceiling(allReviews.Count / (double)pageSize) : 0;
            var pagedItems = allReviews.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(ApiResponse<ReviewsResponse>.Ok(
                new ReviewsResponse(Math.Round(avg, 1), allReviews.Count, breakdown, pagedItems, page, pageSize, totalPages)));
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> CreateReview(int lotId, CreateReviewRequest req)
        {
            if (req.Rating < 1 || req.Rating > 5)
                return BadRequest(ApiResponse<object>.Fail("Rating phải từ 1 đến 5"));

            var lotExists = await _db.ParkingLots.AnyAsync(l => l.ParkingLotId == lotId);
            if (!lotExists) return NotFound(ApiResponse<object>.Fail("Không tìm thấy bãi xe"));

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            var review = new Review
            {
                UserId = userId,
                ParkingLotId = lotId,
                Rating = (byte)req.Rating,
                Comment = req.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();

            return Created("", ApiResponse<object>.Ok(new { review.ReviewId }, "Đã gửi đánh giá"));
        }
    }
}