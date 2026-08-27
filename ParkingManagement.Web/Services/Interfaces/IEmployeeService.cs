using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.ViewModels.Employee;

namespace ParkingManagement.Web.Services.Interfaces;

public interface IEmployeeService
{
    Task<ApiResponse<List<AvailableSlotViewModel>>?> GetAvailableSlotsAsync();
    Task<ApiResponse<List<ActiveSessionViewModel>>?> GetActiveSessionsAsync();
    Task<ApiResponse<CheckInResultViewModel>?> CheckInWithBookingAsync(CheckInWithBookingViewModel model);
    Task<ApiResponse<CheckInResultViewModel>?> CheckInWalkinAsync(CheckInWalkinViewModel model);
    Task<ApiResponse<CheckoutResultViewModel>?> CheckOutAsync(int sessionId);
    Task<ApiResponse<VehicleDashboardViewModel>?> GetVehicleDashboardAsync();
}