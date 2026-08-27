using System.Net.Http.Json;
using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Profile;

namespace ParkingManagement.Web.Services.Implementations;

public class ProfileService : IProfileService
{
    private readonly HttpClient _http;

    public ProfileService(
        IHttpClientFactory factory)
    {
        _http =
            factory.CreateClient("ParkingAPI");
    }


    public async Task<
        ApiResponse<ProfileViewModel>?>
        GetProfileAsync()
    {
        var response =
            await _http.GetAsync("profile/me");

        return await response.Content
            .ReadFromJsonAsync<
                ApiResponse<ProfileViewModel>>();
    }


    public async Task<
        ApiResponse<ProfileViewModel>?>
        UpdateProfileAsync(
            UpdateProfileViewModel model)
    {
        var response =
            await _http.PutAsJsonAsync(
                "profile/me",
                model);

        return await response.Content
            .ReadFromJsonAsync<
                ApiResponse<ProfileViewModel>>();
    }


    public async Task<
        ApiResponse<object>?>
        ChangePasswordAsync(
            ChangePasswordViewModel model)
    {
        var response =
            await _http.PostAsJsonAsync(
                "profile/change-password",
                model);

        return await response.Content
            .ReadFromJsonAsync<
                ApiResponse<object>>();
    }
}