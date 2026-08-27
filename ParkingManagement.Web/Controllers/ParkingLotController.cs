using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Booking;
using ParkingManagement.Web.Services.Implementations;
using Microsoft.AspNetCore.Authorization;

namespace ParkingManagement.Web.Controllers;

public class ParkingLotController : Controller
{
    private readonly IParkingLotService _lotService;
    private readonly IBookingService _bookingService;
    private readonly IReviewService _reviewService; 
    private readonly IAmenityService _amenityService;
    public ParkingLotController(IParkingLotService lotService, IBookingService bookingService, IReviewService reviewService, IAmenityService amenityService)
    {
        _lotService = lotService;
        _bookingService = bookingService;
        _reviewService = reviewService;
        _amenityService = amenityService;

    }

    [HttpGet]
    public async Task<IActionResult> Index(string? keyword)
    {
        var result = await _lotService.SearchAsync(keyword);
        ViewBag.Keyword = keyword;
        return View(result?.Data ?? new());

    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id, int? vehicleTypeId, DateTime? startTime, DateTime? endTime, int reviewPage = 1)
    {
        var lotResult = await _lotService.GetDetailAsync(id);
        var imagesResult = await _lotService.GetImagesAsync(id);
        var reviewsResult = await _reviewService.GetReviewsAsync(id, reviewPage);
        var amenitiesResult = await _amenityService.GetAmenitiesAsync(id);
        if (lotResult == null || !lotResult.Success || lotResult.Data == null)
            return NotFound();

        var typesResult = await _lotService.GetVehicleTypesAsync();
     
        var vm = new ParkingLotDetailPageViewModel
        {
            Lot = lotResult.Data,
            VehicleTypes = typesResult?.Data ?? new(),
            SelectedVehicleTypeId = vehicleTypeId,
            StartTime = startTime,
            EndTime = endTime,
            Images = imagesResult?.Data ?? new(),
            Reviews = reviewsResult?.Data ?? new ReviewsViewModel()
        };

        if (vehicleTypeId.HasValue && startTime.HasValue && endTime.HasValue)
        {
            var availResult = await _bookingService.CheckAvailabilityAsync(
                id, vehicleTypeId.Value, startTime.Value, endTime.Value);

            vm.Availability = availResult?.Data;
            vm.AvailabilityMessage = availResult?.Success == false ? availResult.Message : null;
        }
        var statsResult = await _lotService.GetQuickStatsAsync(id);

        vm.QuickStats = statsResult?.Data;

        vm.Amenities =
            amenitiesResult?.Data ?? new();

        vm.Reviews =
            reviewsResult?.Data ?? new ReviewsViewModel();

        ViewBag.Error = TempData["Error"];
        ViewBag.SuccessMessage = TempData["SuccessMessage"];

        return View(vm);
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> SubmitReview(CreateReviewViewModel model)
    {
        var result = await _reviewService.CreateReviewAsync(model);

        TempData[result?.Success == true ? "SuccessMessage" : "Error"] =
            result?.Success == true ? "Cảm ơn bạn đã đánh giá!" : (result?.Message ?? "Gửi đánh giá thất bại");

        return RedirectToAction("Detail", new { id = model.ParkingLotId });
    }
}