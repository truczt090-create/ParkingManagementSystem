using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Owner;

namespace ParkingManagement.Web.Controllers;

public class OwnerController : Controller
{
    private readonly IOwnerService _ownerService;
    public OwnerController(IOwnerService ownerService) => _ownerService = ownerService;

    private bool RequireOwner(out IActionResult? redirect)
    {
        if (HttpContext.Session.GetString("Role") != "Owner")
        {
            redirect = RedirectToAction("Login", "Account");
            return false;
        }
        redirect = null;
        return true;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard(string range = "day")
    {
        if (!RequireOwner(out var redirect))
            return redirect!;

        var revenue = await _ownerService.GetRevenueAsync(range);
        var occupancy = await _ownerService.GetOccupancyAsync();
        var summary = await _ownerService.GetDashboardSummaryAsync();

        ViewBag.Range = range;

        ViewBag.Occupancy =
            occupancy?.Data ?? new OccupancyViewModel();

        ViewBag.Summary =
            summary?.Data ?? new OwnerDashboardSummaryViewModel();

        return View(
            revenue?.Data ?? new RevenueViewModel()
        );
    }
    [HttpGet]
    public async Task<IActionResult> MyLots()
    {
        if (!RequireOwner(out var redirect)) return redirect!;

        var result = await _ownerService.GetMyLotsAsync();
        return View(result?.Data ?? new List<OwnerLotViewModel>());
    }

    [HttpGet]
    public async Task<IActionResult> LotDetail(int lotId)
    {
        if (!RequireOwner(out var redirect)) return redirect!;

        var areas = await _ownerService.GetAreasAsync(lotId);
        var prices = await _ownerService.GetPricesAsync(lotId);
        var images = await _ownerService.GetLotImagesAsync(lotId);

        ViewBag.ParkingLotId = lotId;
        ViewBag.Prices = prices?.Data ?? new List<PriceViewModel>();
        ViewBag.Error = TempData["Error"];
        ViewBag.SuccessMessage = TempData["SuccessMessage"];
        ViewBag.Images = images?.Data ?? new List<ParkingLotImageViewModel>();

        return View(areas?.Data ?? new List<AreaViewModel>());
    }

    [HttpPost]
    public async Task<IActionResult> CreateArea(CreateAreaViewModel model)
    {
        if (!RequireOwner(out var redirect)) return redirect!;

        var result = await _ownerService.CreateAreaAsync(model);
        TempData[result?.Success == true ? "SuccessMessage" : "Error"] =
            result?.Success == true ? "Tạo khu vực thành công" : (result?.Message ?? "Lỗi");

        return RedirectToAction("LotDetail", new { lotId = model.ParkingLotId });
    }

    [HttpGet]
    public async Task<IActionResult> AreaSlots(int areaId, int lotId)
    {
        if (!RequireOwner(out var redirect))
            return redirect!;

        // Không cho truy cập khu vực ID = 0
        if (areaId <= 0)
        {
            TempData["Error"] =
                "Vui lòng chọn một khu vực trước khi quản lý vị trí đỗ.";

            return RedirectToAction(
                "LotDetail",
                new { lotId = lotId }
            );
        }

        var slots = await _ownerService.GetSlotsAsync(areaId);

        // API báo lỗi / không tìm thấy khu vực
        if (slots == null || !slots.Success)
        {
            TempData["Error"] =
                slots?.Message ?? "Không tìm thấy khu vực.";

            return RedirectToAction(
                "LotDetail",
                new { lotId = lotId }
            );
        }

        ViewBag.ParkingAreaId = areaId;
        ViewBag.ParkingLotId = lotId;
        ViewBag.Error = TempData["Error"];
        ViewBag.SuccessMessage = TempData["SuccessMessage"];

        return View(
            slots.Data ?? new List<SlotViewModel>()
        );
    } 

    [HttpPost]
    public async Task<IActionResult> CreateSlot(CreateSlotViewModel model, int lotId)
    {
        if (!RequireOwner(out var redirect)) return redirect!;

        var result = await _ownerService.CreateSlotAsync(model);
        TempData[result?.Success == true ? "SuccessMessage" : "Error"] =
            result?.Success == true ? "Tạo vị trí đỗ thành công" : (result?.Message ?? "Lỗi");

        return RedirectToAction("AreaSlots", new { areaId = model.ParkingAreaId, lotId });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBulkSlots(
     CreateBulkSlotsViewModel model,
     int lotId)
    {
        if (!RequireOwner(out var redirect))
            return redirect!;

        // Kiểm tra khu vực có hợp lệ không
        if (model.ParkingAreaId <= 0)
        {
            TempData["Error"] =
                "Khu vực không hợp lệ.";

            return RedirectToAction(
                "LotDetail",
                new { lotId }
            );
        }

        // Kiểm tra số lượng
        if (model.Quantity <= 0)
        {
            TempData["Error"] =
                "Số lượng chỗ phải lớn hơn 0.";

            return RedirectToAction(
                "AreaSlots",
                new
                {
                    areaId = model.ParkingAreaId,
                    lotId
                });
        }

        // Gọi service tạo hàng loạt
        var result =
            await _ownerService.CreateBulkSlotsAsync(model);

        TempData[
            result?.Success == true
                ? "SuccessMessage"
                : "Error"
        ] =
            result?.Success == true
                ? $"Đã tạo {model.Quantity} vị trí đỗ."
                : result?.Message
                    ?? "Tạo vị trí đỗ thất bại.";

        return RedirectToAction(
            "AreaSlots",
            new
            {
                areaId = model.ParkingAreaId,
                lotId
            });
    }
    [HttpPost]
    public async Task<IActionResult> CreatePrice(CreatePriceViewModel model)
    {
        if (!RequireOwner(out var redirect)) return redirect!;

        var result = await _ownerService.CreatePriceAsync(model);
        TempData[result?.Success == true ? "SuccessMessage" : "Error"] =
            result?.Success == true ? "Thêm giá thành công" : (result?.Message ?? "Lỗi");

        return RedirectToAction("LotDetail", new { lotId = model.ParkingLotId });
    }
    [HttpGet]
    public async Task<IActionResult> Employees(int lotId)
    {
        if (!RequireOwner(out var redirect)) return redirect!;

        var result = await _ownerService.GetEmployeesAsync(lotId);
        ViewBag.ParkingLotId = lotId;
        ViewBag.Error = TempData["Error"];
        ViewBag.SuccessMessage = TempData["SuccessMessage"];

        return View(result?.Data ?? new());
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee(CreateEmployeeViewModel model)
    {
        if (!RequireOwner(out var redirect)) return redirect!;

        var result = await _ownerService.CreateEmployeeAsync(model);
        TempData[result?.Success == true ? "SuccessMessage" : "Error"] =
            result?.Success == true ? "Đã tạo tài khoản nhân viên" : (result?.Message ?? "Lỗi");

        return RedirectToAction("Employees", new { lotId = model.ParkingLotId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveEmployee(int employeeId, int lotId)
    {
        if (!RequireOwner(out var redirect)) return redirect!;

        await _ownerService.RemoveEmployeeAsync(employeeId);
        TempData["SuccessMessage"] = "Đã xóa nhân viên";
        return RedirectToAction("Employees", new { lotId });
    }
    [HttpGet]
    public IActionResult CreateLot()
    {
        if (!RequireOwner(out var redirect))
            return redirect!;

        return View(new CreateParkingLotViewModel());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateLot(
    CreateParkingLotViewModel model)
    {
        if (!RequireOwner(out var redirect))
            return redirect!;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _ownerService.CreateLotAsync(model);

        if (result == null ||
            !result.Success ||
            result.Data == null)
        {
            ModelState.AddModelError(
                "",
                result?.Message ?? "Đăng ký bãi xe thất bại");

            return View(model);
        }

        var lotId = result.Data.ParkingLotId;

        // Lưu các tiện ích đã chọn
        foreach (var amenity in model.Amenities
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .Distinct())
        {
            await _ownerService.AddAmenityAsync(
                lotId,
                amenity);
        }

        TempData["SuccessMessage"] =
            "Đăng ký bãi xe thành công. Bãi xe đang chờ Admin duyệt.";

        return RedirectToAction("MyLots");
    }
    [HttpGet]
    public async Task<IActionResult> EditLot(int lotId)
    {
        if (!RequireOwner(out var redirect))
            return redirect!;

        var result = await _ownerService.GetLotForEditAsync(lotId);

        if (result == null ||
            !result.Success ||
            result.Data == null)
        {
            TempData["Error"] =
                result?.Message ?? "Không tìm thấy bãi xe";

            return RedirectToAction("MyLots");
        }

        return View(result.Data);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditLot(
    EditParkingLotViewModel model)
    {
        if (!RequireOwner(out var redirect))
            return redirect!;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _ownerService.UpdateLotAsync(model);

        if (result == null || !result.Success)
        {
            ModelState.AddModelError(
                "",
                result?.Message ?? "Cập nhật bãi xe thất bại");

            return View(model);
        }

        TempData["SuccessMessage"] =
            "Cập nhật thông tin bãi xe thành công.";

        return RedirectToAction("MyLots");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadLotImage(
    int parkingLotId,
    IFormFile image,
    bool isPrimary = false)
    {
        if (!RequireOwner(out var redirect))
            return redirect!;

        if (image == null || image.Length == 0)
        {
            TempData["Error"] = "Vui lòng chọn ảnh.";

            return RedirectToAction(
                "LotDetail",
                new { lotId = parkingLotId });
        }

        var allowedExtensions = new[]
        {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

        var extension = Path
            .GetExtension(image.FileName)
            .ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            TempData["Error"] =
                "Chỉ hỗ trợ JPG, JPEG, PNG hoặc WEBP.";

            return RedirectToAction(
                "LotDetail",
                new { lotId = parkingLotId });
        }

        const long maxFileSize =
            5 * 1024 * 1024;

        if (image.Length > maxFileSize)
        {
            TempData["Error"] =
                "Ảnh không được lớn hơn 5MB.";

            return RedirectToAction(
                "LotDetail",
                new { lotId = parkingLotId });
        }

        // Tạo folder cho từng bãi
        var folder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "uploads",
            "parkinglots",
            parkingLotId.ToString()
        );

        Directory.CreateDirectory(folder);

        // Không dùng tên file do user gửi trực tiếp
        var fileName =
            $"{Guid.NewGuid():N}{extension}";

        var physicalPath =
            Path.Combine(folder, fileName);

        await using (var stream =
            new FileStream(
                physicalPath,
                FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        var imageUrl =
            $"/uploads/parkinglots/{parkingLotId}/{fileName}";

        var result = await _ownerService.AddLotImageAsync(
            parkingLotId,
            imageUrl,
            isPrimary
        );

        if (result == null || !result.Success)
        {
            // API không lưu được thì xóa file vừa upload
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            TempData["Error"] =
                result?.Message ??
                "Upload ảnh thất bại.";
        }
        else
        {
            TempData["SuccessMessage"] =
                "Thêm ảnh bãi xe thành công.";
        }

        return RedirectToAction(
            "LotDetail",
            new { lotId = parkingLotId });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLotImage(
    int parkingLotId,
    int imageId)
    {
        if (!RequireOwner(out var redirect))
            return redirect!;

        var result =
            await _ownerService.DeleteLotImageAsync(
                parkingLotId,
                imageId);

        TempData[
            result?.Success == true
                ? "SuccessMessage"
                : "Error"
        ] =
            result?.Success == true
                ? "Đã xóa ảnh."
                : result?.Message ?? "Xóa ảnh thất bại.";

        return RedirectToAction(
            "LotDetail",
            new { lotId = parkingLotId });
    }
}