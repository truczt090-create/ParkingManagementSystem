using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Employee;


namespace ParkingManagement.Web.Controllers;

public class EmployeeController : Controller
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    private bool RequireEmployee(out IActionResult? redirect)
    {
        var role = HttpContext.Session.GetString("Role");
        if (role != "Employee")
        {
            redirect = RedirectToAction("Login", "Account");
            return false;
        }
        redirect = null;
        return true;
    }

    // TRANG CHECK-IN 

    public async Task<IActionResult> CheckIn(int? parkingSlotId)
    {
        if (!RequireEmployee(out var redirect))
            return redirect!;
  
        var result = await _employeeService.GetAvailableSlotsAsync();

        ViewBag.AvailableSlots =
            result?.Data ?? new List<AvailableSlotViewModel>();

        ViewBag.SelectedParkingSlotId = parkingSlotId;

        ViewBag.Error = TempData["Error"];
        ViewBag.SuccessMessage = TempData["SuccessMessage"];

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CheckInWithBooking(CheckInWithBookingViewModel model)
    {
        if (!RequireEmployee(out var redirect)) return redirect!;

        var result = await _employeeService.CheckInWithBookingAsync(model);

        if (result == null || !result.Success)
            TempData["Error"] = result?.Message ?? "Check-in thất bại";
        else
            TempData["SuccessMessage"] = $"Check-in thành công — vị trí {result.Data?.SlotCode}";

        return RedirectToAction("CheckIn");
    }

    [HttpPost]
    public async Task<IActionResult> CheckInWalkin(CheckInWalkinViewModel model)
    {
        if (!RequireEmployee(out var redirect)) return redirect!;

        var result = await _employeeService.CheckInWalkinAsync(model);

        if (result == null || !result.Success)
            TempData["Error"] = result?.Message ?? "Check-in vãng lai thất bại";
        else
            TempData["SuccessMessage"] = $"Check-in vãng lai thành công — vị trí {result.Data?.SlotCode}";

        return RedirectToAction("CheckIn");
    }

    //  TRANG CHECK-OUT 

    [HttpGet]
    public async Task<IActionResult> CheckOut()
    {
        if (!RequireEmployee(out var redirect)) return redirect!;

        var sessionsResult = await _employeeService.GetActiveSessionsAsync();
        ViewBag.CheckoutResult = TempData["CheckoutResult"];

        return View(sessionsResult?.Data ?? new List<ActiveSessionViewModel>());
    }

    [HttpPost]
    public async Task<IActionResult> DoCheckOut(int sessionId)
    {
        if (!RequireEmployee(out var redirect)) return redirect!;

        var result = await _employeeService.CheckOutAsync(sessionId);

        if (result == null || !result.Success || result.Data == null)
        {
            TempData["Error"] = result?.Message ?? "Check-out thất bại";
            return RedirectToAction("CheckOut");
        }

        var data = result.Data;
        var msg = data.AmountToCollect > 0
            ? $"Thu {data.AmountToCollect:N0}đ tiền mặt" +
              (data.IsOvertime ? $" (quá giờ {data.OvertimeHours} giờ)" : " (khách vãng lai)")
            : "Checkout thành công, không phát sinh phí";

        TempData["CheckoutResult"] = msg;
        return RedirectToAction("CheckOut");
    }
    public async Task<IActionResult> VehicleDashboard()
    {
        var result =
            await _employeeService.GetVehicleDashboardAsync();

        if (result?.Success != true || result.Data == null)
        {
            ViewBag.Error =
                result?.Message ?? "Không tải được sơ đồ bãi xe.";

            return View(new VehicleDashboardViewModel());
        }

        return View(result.Data);
    }
}