using ParkingManagement.API.DTOs.Profile;

namespace ParkingManagement.API.Services.Interfaces;

public interface IProfileService
{
    Task<ProfileResponse> GetProfileAsync(int userId);

    Task<ProfileResponse> UpdateProfileAsync(
        int userId,
        UpdateProfileRequest request);

    Task ChangePasswordAsync(
        int userId,
        ChangePasswordRequest request);
}