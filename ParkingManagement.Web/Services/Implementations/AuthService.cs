using System.Net.Http.Json;
using ParkingManagement.Web.Models.Auth;
using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Auth;

namespace ParkingManagement.Web.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;

    public AuthService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("ParkingAPI");
    }

    public async Task<ApiResponse<AuthResponse>?> LoginAsync(LoginViewModel model)
    {
        var response = await _http.PostAsJsonAsync("auth/login", model);
        return await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
    }

    public async Task<ApiResponse<object>?> RegisterAsync(RegisterViewModel model)
    {
        var response = await _http.PostAsJsonAsync("auth/register", model);
        return await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
    }
    public async Task<ApiResponse<object>?> RegisterOwnerAsync(RegisterOwnerViewModel model)
    {
        var response = await _http.PostAsJsonAsync("auth/register-owner", model);
        return await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
    }
}