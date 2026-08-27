using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.ViewModels.Profile;

namespace ParkingManagement.Web.Services.Interfaces;

public interface IProfileService
{
    Task<ApiResponse<ProfileViewModel>?>
        GetProfileAsync();

    Task<ApiResponse<ProfileViewModel>?>
        UpdateProfileAsync(
            UpdateProfileViewModel model);

    Task<ApiResponse<object>?>
        ChangePasswordAsync(
            ChangePasswordViewModel model);
}