using ParkingManagement.Web.Models.Auth;
using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.ViewModels.Auth;

namespace ParkingManagement.Web.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>?> LoginAsync(LoginViewModel model);
    Task<ApiResponse<object>?> RegisterAsync(RegisterViewModel model);
    // Interface
    Task<ApiResponse<object>?> RegisterOwnerAsync(RegisterOwnerViewModel model);
}