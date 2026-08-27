using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Booking;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Booking;

namespace ParkingManagement.Web.Controllers;

public class BookingController : Controller
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingViewModel model)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            return RedirectToAction("Login", "Account");

        var result = await _bookingService.CreateBookingAsync(model);

        if (result == null || !result.Success || result.Data == null)
        {
            TempData["Error"] = result?.Message ?? "Đặt chỗ thất bại";
            return RedirectToAction("Detail", "ParkingLot", new
            {
                id = model.ParkingLotId,
                vehicleTypeId = model.VehicleTypeId,
                startTime = model.StartTime,
                endTime = model.EndTime
            });
        }

        return RedirectToAction("Pay", new { bookingId = result.Data.BookingId, amount = result.Data.TotalAmount });
    }

    [HttpGet]
    public IActionResult Pay(int bookingId, decimal amount)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            return RedirectToAction("Login", "Account");

        return View(new PayBookingViewModel { BookingId = bookingId, TotalAmount = amount });
    }

    [HttpPost]
    public async Task<IActionResult> Pay(PayBookingViewModel model)
    {
        var result = await _bookingService.PayBookingAsync(model.BookingId, model.PaymentMethod);

        if (result == null || !result.Success)
        {
            ModelState.AddModelError("", result?.Message ?? "Thanh toán thất bại");
            return View(model);
        }

        return RedirectToAction("MyBookings");
    }

    [HttpGet]
    public async Task<IActionResult> MyBookings()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            return RedirectToAction("Login", "Account");

        var result = await _bookingService.GetMyBookingsAsync();
        return View(result?.Data ?? new());
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        await _bookingService.CancelBookingAsync(id);
        return RedirectToAction("MyBookings");
    }

    [HttpPost]
    public async Task<IActionResult> Extend(ExtendBookingViewModel model)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            return RedirectToAction("Login", "Account");

        var result = await _bookingService.ExtendBookingAsync(model);

        TempData[result?.Success == true ? "SuccessMessage" : "Error"] =
            result?.Success == true
                ? $"Gia hạn thành công, thêm {result.Data?.AdditionalAmount:N0}đ"
                : (result?.Message ?? "Gia hạn thất bại");

        return RedirectToAction("MyBookings");
    }
}
