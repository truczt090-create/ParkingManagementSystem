using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.ViewModels.Admin;

namespace ParkingManagement.Web.Services.Interfaces;

public interface IAdminService
{
    Task<ApiResponse<AdminDashboardViewModel>?> GetDashboardAsync();
    Task<ApiResponse<List<PendingLotViewModel>>?> GetLotsByStatusAsync(string status);
    Task<ApiResponse<object>?> ApproveLotAsync(int id);
    Task<ApiResponse<object>?> RejectLotAsync(int id);
    Task<ApiResponse<List<AdminUserViewModel>>?> GetUsersAsync(string? role);
    Task<ApiResponse<object>?> ToggleUserActiveAsync(int id);
}