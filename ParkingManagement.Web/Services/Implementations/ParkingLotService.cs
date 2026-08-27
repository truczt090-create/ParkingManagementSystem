using System.Net.Http.Json;
using System.Text.Json;
using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.Models.ParkingLot;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Booking;

namespace ParkingManagement.Web.Services.Implementations;

public class ParkingLotService : IParkingLotService
{
    private readonly HttpClient _http;

    public ParkingLotService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("ParkingAPI");
    }


    // =====================================================
    // TÌM DANH SÁCH BÃI XE
    // =====================================================

    public async Task<ApiResponse<List<ParkingLotViewModel>>?> SearchAsync(
        string? keyword)
    {
        var url = "parkinglots";

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            url += $"?keyword={Uri.EscapeDataString(keyword)}";
        }

        var response = await _http.GetAsync(url);

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Không gọi được API tìm bãi xe.\n" +
                $"Status: {(int)response.StatusCode} {response.StatusCode}\n" +
                $"URL: {_http.BaseAddress}{url}\n" +
                $"Response: {content}"
            );
        }

        try
        {
            return JsonSerializer.Deserialize<
                ApiResponse<List<ParkingLotViewModel>>
            >(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        }
        catch (JsonException)
        {
            throw new Exception(
                $"API có phản hồi nhưng dữ liệu không phải JSON hợp lệ.\n" +
                $"URL: {_http.BaseAddress}{url}\n" +
                $"Response thực tế: {content}"
            );
        }
    }


    // =====================================================
    // CHI TIẾT BÃI XE
    // =====================================================

    public async Task<ApiResponse<ParkingLotViewModel>?> GetDetailAsync(int id)
    {
        var url = $"parkinglots/{id}";

        var response = await _http.GetAsync(url);

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Không lấy được chi tiết bãi xe.\n" +
                $"Status: {(int)response.StatusCode} {response.StatusCode}\n" +
                $"URL: {_http.BaseAddress}{url}\n" +
                $"Response: {content}"
            );
        }

        return JsonSerializer.Deserialize<
            ApiResponse<ParkingLotViewModel>
        >(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        );
    }


    // =====================================================
    // LOẠI XE
    // =====================================================

    public async Task<ApiResponse<List<VehicleTypeViewModel>>?>
        GetVehicleTypesAsync()
    {
        const string url = "vehicletypes";

        var response = await _http.GetAsync(url);

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Không lấy được loại xe.\n" +
                $"Status: {(int)response.StatusCode} {response.StatusCode}\n" +
                $"URL: {_http.BaseAddress}{url}\n" +
                $"Response: {content}"
            );
        }

        return JsonSerializer.Deserialize<
            ApiResponse<List<VehicleTypeViewModel>>
        >(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        );
    }
    // Implementation
    public async Task<ApiResponse<QuickStatsViewModel>?> GetQuickStatsAsync(int lotId)
    {
        var response = await _http.GetAsync($"parkinglots/{lotId}/stats");
        return await response.Content.ReadFromJsonAsync<ApiResponse<QuickStatsViewModel>>();
    }
    public async Task<ApiResponse<List<ParkingLotImageViewModel>>?> GetImagesAsync(int parkingLotId)
    {
        var response = await _http.GetAsync($"parkinglots/{parkingLotId}/images");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content
            .ReadFromJsonAsync<ApiResponse<List<ParkingLotImageViewModel>>>();
    }
}