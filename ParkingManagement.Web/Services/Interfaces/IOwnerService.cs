using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.ViewModels.Owner;

namespace ParkingManagement.Web.Services.Interfaces;

public interface IOwnerService
{
    Task<ApiResponse<RevenueViewModel>?> GetRevenueAsync(string range);
    Task<ApiResponse<OccupancyViewModel>?> GetOccupancyAsync();
    Task<ApiResponse<List<OwnerLotViewModel>>?> GetMyLotsAsync();
    Task<ApiResponse<List<AreaViewModel>>?> GetAreasAsync(int lotId);
    Task<ApiResponse<object>?> CreateAreaAsync(CreateAreaViewModel model);
    Task<ApiResponse<List<SlotViewModel>>?> GetSlotsAsync(int areaId);
    Task<ApiResponse<object>?> CreateSlotAsync(CreateSlotViewModel model);
    Task<ApiResponse<List<PriceViewModel>>?> GetPricesAsync(int lotId);
    Task<ApiResponse<object>?> CreatePriceAsync(CreatePriceViewModel model);
    Task<ApiResponse<List<EmployeeSummaryViewModel>>?> GetEmployeesAsync(int lotId);
    Task<ApiResponse<object>?> CreateEmployeeAsync(CreateEmployeeViewModel model);
    Task<ApiResponse<object>?> RemoveEmployeeAsync(int employeeId);
    Task<ApiResponse<CreateParkingLotResultViewModel>?>
    CreateLotAsync(CreateParkingLotViewModel model);

    Task<ApiResponse<object>?>
        AddAmenityAsync(int lotId, string content);
    Task<ApiResponse<EditParkingLotViewModel>?> GetLotForEditAsync(int lotId);

    Task<ApiResponse<object>?> UpdateLotAsync(EditParkingLotViewModel model);

    Task<ApiResponse<List<ParkingLotImageViewModel>>?>
    GetLotImagesAsync(int lotId);

    Task<ApiResponse<object>?>
        AddLotImageAsync(
            int lotId,
            string imageUrl,
            bool isPrimary);

    Task<ApiResponse<object>?>
        DeleteLotImageAsync(
            int lotId,
            int imageId);
    Task<ApiResponse<OwnerDashboardSummaryViewModel>?> GetDashboardSummaryAsync();
    Task<ApiResponse<object>?> CreateBulkSlotsAsync(CreateBulkSlotsViewModel model);
}
