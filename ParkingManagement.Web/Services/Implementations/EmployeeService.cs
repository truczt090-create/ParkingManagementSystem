using System.Net.Http;
using System.Net.Http.Json;
using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Employee;

namespace ParkingManagement.Web.Services.Implementations;

public class EmployeeService : IEmployeeService
{
    private readonly HttpClient _http;

    public EmployeeService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("ParkingAPI");
    }

    public async Task<ApiResponse<List<AvailableSlotViewModel>>?> GetAvailableSlotsAsync()
    {
        var response = await _http.GetAsync("employee/slots");
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<AvailableSlotViewModel>>>();
    }

    public async Task<ApiResponse<List<ActiveSessionViewModel>>?> GetActiveSessionsAsync()
    {
        var response = await _http.GetAsync("sessions/active");
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<ActiveSessionViewModel>>>();
    }

    public async Task<ApiResponse<CheckInResultViewModel>?> CheckInWithBookingAsync(
    CheckInWithBookingViewModel model)
    {
        var response = await _http.PostAsJsonAsync(
            "checkin/with-booking",
            model
        );

        var rawContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"CHECK-IN API trả lỗi {(int)response.StatusCode} " +
                $"{response.StatusCode}. Nội dung: {rawContent}"
            );
        }

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<
                ApiResponse<CheckInResultViewModel>
            >(
                rawContent,
                new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        }
        catch (System.Text.Json.JsonException)
        {
            throw new Exception(
                $"CHECK-IN API không trả JSON. Nội dung thực tế: {rawContent}"
            );
        }
    }

    public async Task<ApiResponse<CheckInResultViewModel>?> CheckInWalkinAsync(CheckInWalkinViewModel model)
    {
        var response = await _http.PostAsJsonAsync("checkin/walkin", model);
        return await response.Content.ReadFromJsonAsync<ApiResponse<CheckInResultViewModel>>();
    }
    public async Task<ApiResponse<CheckoutResultViewModel>?> CheckOutAsync(int sessionId)
    {
        var response =
            await _http.PostAsync($"checkout/{sessionId}", null);

        return await response.Content
            .ReadFromJsonAsync<ApiResponse<CheckoutResultViewModel>>();
    }
    //public async Task<List<ParkingSlotDashboardViewModel>> GetParkingSlotsAsync(int parkingLotId)
    //{
    //    var response = await _http.GetAsync($"parkinglots/{parkingLotId}/slots");

    //    if (!response.IsSuccessStatusCode)
    //        return new List<ParkingSlotDashboardViewModel>();

    //    var result = await response.Content
    //        .ReadFromJsonAsync<List<ParkingSlotDashboardViewModel>>();

    //    return result ?? new List<ParkingSlotDashboardViewModel>();
    //}
    public async Task<ApiResponse<VehicleDashboardViewModel>?>
    GetVehicleDashboardAsync()
    {
        var response =
            await _http.GetAsync("employee/vehicle-dashboard");

        return await response.Content
            .ReadFromJsonAsync<
                ApiResponse<VehicleDashboardViewModel>>();
    }
}