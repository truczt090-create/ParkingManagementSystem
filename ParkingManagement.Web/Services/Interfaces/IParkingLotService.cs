using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.Models.ParkingLot;
using ParkingManagement.Web.ViewModels.Booking;
namespace ParkingManagement.Web.Services.Interfaces;

public interface IParkingLotService
{
    Task<ApiResponse<List<ParkingLotViewModel>>?> SearchAsync(string? keyword);
    Task<ApiResponse<ParkingLotViewModel>?> GetDetailAsync(int id);
    Task<ApiResponse<List<VehicleTypeViewModel>>?> GetVehicleTypesAsync();
    // Interface
    Task<ApiResponse<QuickStatsViewModel>?> GetQuickStatsAsync(int lotId);
    Task<ApiResponse<List<ParkingLotImageViewModel>>?> GetImagesAsync(int parkingLotId);
}