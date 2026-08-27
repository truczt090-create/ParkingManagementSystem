using System.Net.Http.Json;
using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Admin;

namespace ParkingManagement.Web.Services.Implementations;

public class AdminService : IAdminService
{
    private readonly HttpClient _http;
    public AdminService(IHttpClientFactory factory) => _http = factory.CreateClient("ParkingAPI");

    public async Task<ApiResponse<AdminDashboardViewModel>?> GetDashboardAsync()
    {
        var response = await _http.GetAsync("admin/dashboard");
        return await response.Content.ReadFromJsonAsync<ApiResponse<AdminDashboardViewModel>>();
    }

    public async Task<ApiResponse<List<PendingLotViewModel>>?> GetLotsByStatusAsync(string status)
    {
        var response = await _http.GetAsync($"admin/parkinglots?status={status}");
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<PendingLotViewModel>>>();
    }

    public async Task<ApiResponse<object>?> ApproveLotAsync(int id)
    {
        var response = await _http.PostAsync($"admin/parkinglots/{id}/approve", null);
        return await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
    }

    public async Task<ApiResponse<object>?> RejectLotAsync(int id)
    {
        var response = await _http.PostAsync($"admin/parkinglots/{id}/reject", null);
        return await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
    }

    public async Task<ApiResponse<List<AdminUserViewModel>>?> GetUsersAsync(string? role)
    {
        var url = "admin/users" + (string.IsNullOrEmpty(role) ? "" : $"?role={role}");
        var response = await _http.GetAsync(url);
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<AdminUserViewModel>>>();
    }

    public async Task<ApiResponse<object>?> ToggleUserActiveAsync(int id)
    {
        var response = await _http.PostAsync($"admin/users/{id}/toggle-active", null);
        return await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
    }
} 