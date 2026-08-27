using System.Net.Http.Json;
using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Owner;

namespace ParkingManagement.Web.Services.Implementations;

public class OwnerService : IOwnerService
{
    private readonly HttpClient _http;
    public OwnerService(IHttpClientFactory factory) => _http = factory.CreateClient("ParkingAPI");

    public async Task<ApiResponse<RevenueViewModel>?> GetRevenueAsync(string range)
    {
        var response = await _http.GetAsync($"owner/dashboard/revenue?range={range}");
        return await response.Content.ReadFromJsonAsync<ApiResponse<RevenueViewModel>>();
    }

    public async Task<ApiResponse<OccupancyViewModel>?> GetOccupancyAsync()
    {
        var response = await _http.GetAsync("owner/dashboard/occupancy");
        return await response.Content.ReadFromJsonAsync<ApiResponse<OccupancyViewModel>>();
    }

    public async Task<ApiResponse<List<OwnerLotViewModel>>?> GetMyLotsAsync()
    {
        var response = await _http.GetAsync("owner/parkinglots/my");
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<OwnerLotViewModel>>>();
    }

    public async Task<ApiResponse<List<AreaViewModel>>?> GetAreasAsync(int lotId)
    {
        var response = await _http.GetAsync($"owner/parkinglots/{lotId}/areas");
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<AreaViewModel>>>();
    }

    public async Task<ApiResponse<object>?> CreateAreaAsync(CreateAreaViewModel model)
    {
        var response = await _http.PostAsJsonAsync($"owner/parkinglots/{model.ParkingLotId}/areas", new { model.Name });
        return await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
    }

    public async Task<ApiResponse<List<SlotViewModel>>?> GetSlotsAsync(int areaId)
    {
        var response = await _http.GetAsync($"owner/areas/{areaId}/slots");
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<SlotViewModel>>>();
    }

    public async Task<ApiResponse<object>?> CreateSlotAsync(CreateSlotViewModel model)
    {
        var response = await _http.PostAsJsonAsync($"owner/areas/{model.ParkingAreaId}/slots",
            new { model.VehicleTypeId, model.SlotCode });
        return await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
    }

    public async Task<ApiResponse<List<PriceViewModel>>?> GetPricesAsync(int lotId)
    {
        var response = await _http.GetAsync($"owner/prices?parkingLotId={lotId}");
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<PriceViewModel>>>();
    }

    public async Task<ApiResponse<object>?> CreatePriceAsync(CreatePriceViewModel model)
    {
        var response = await _http.PostAsJsonAsync("owner/prices", model);
        return await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
    }
    public async Task<ApiResponse<List<EmployeeSummaryViewModel>>?> GetEmployeesAsync(int lotId)
    {
        var response = await _http.GetAsync($"owner/employees?parkingLotId={lotId}");
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<EmployeeSummaryViewModel>>>();
    }

    public async Task<ApiResponse<object>?> CreateEmployeeAsync(CreateEmployeeViewModel model)
    {
        var response = await _http.PostAsJsonAsync($"owner/employees?parkingLotId={model.ParkingLotId}",
            new { model.FullName, model.Email, model.Password, model.Phone, model.Shift });
        return await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
    }

    public async Task<ApiResponse<object>?> RemoveEmployeeAsync(int employeeId)
    {
        var response = await _http.DeleteAsync($"owner/employees/{employeeId}");
        return await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
    }
    public async Task<ApiResponse<CreateParkingLotResultViewModel>?>
    CreateLotAsync(CreateParkingLotViewModel model)
    {
        var request = new
        {
            model.Name,
            model.Address,
            model.Latitude,
            model.Longitude,
            model.OpenTime,
            model.CloseTime
        };

        var response = await _http.PostAsJsonAsync(
            "owner/parkinglots",
            request);

        return await response.Content
            .ReadFromJsonAsync<
                ApiResponse<CreateParkingLotResultViewModel>>();
    }


    public async Task<ApiResponse<object>?>
        AddAmenityAsync(int lotId, string content)
    {
        var response = await _http.PostAsJsonAsync(
            $"owner/parkinglots/{lotId}/amenities",
            new
            {
                Content = content
            });

        return await response.Content
            .ReadFromJsonAsync<ApiResponse<object>>();
    }
    public async Task<ApiResponse<EditParkingLotViewModel>?>
    GetLotForEditAsync(int lotId)
    {
        var response = await _http.GetAsync(
            $"owner/parkinglots/{lotId}");

        return await response.Content
            .ReadFromJsonAsync<ApiResponse<EditParkingLotViewModel>>();
    }


    public async Task<ApiResponse<object>?>
        UpdateLotAsync(EditParkingLotViewModel model)
    {
        var request = new
        {
            model.Name,
            model.Address,
            model.Latitude,
            model.Longitude,
            model.OpenTime,
            model.CloseTime
        };

        var response = await _http.PutAsJsonAsync(
            $"owner/parkinglots/{model.ParkingLotId}",
            request);

        return await response.Content
            .ReadFromJsonAsync<ApiResponse<object>>();
    }
    public async Task<ApiResponse<List<ParkingLotImageViewModel>>?>
    GetLotImagesAsync(int lotId)
    {
        var response = await _http.GetAsync(
            $"parkinglots/{lotId}/images");

        return await response.Content
            .ReadFromJsonAsync<
                ApiResponse<List<ParkingLotImageViewModel>>>();
    }


    public async Task<ApiResponse<object>?>
        AddLotImageAsync(
            int lotId,
            string imageUrl,
            bool isPrimary)
    {
        var response = await _http.PostAsJsonAsync(
            $"owner/parkinglots/{lotId}/images",
            new
            {
                ImageUrl = imageUrl,
                IsPrimary = isPrimary
            });

        return await response.Content
            .ReadFromJsonAsync<ApiResponse<object>>();
    }


    public async Task<ApiResponse<object>?>
        DeleteLotImageAsync(
            int lotId,
            int imageId)
    {
        var response = await _http.DeleteAsync(
            $"owner/parkinglots/{lotId}/images/{imageId}");

        return await response.Content
            .ReadFromJsonAsync<ApiResponse<object>>();
    }
    public async Task<ApiResponse<OwnerDashboardSummaryViewModel>?> GetDashboardSummaryAsync()
    {
        var response = await _http.GetAsync("owner/dashboard/summary");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content
            .ReadFromJsonAsync<ApiResponse<OwnerDashboardSummaryViewModel>>();
    }
    public async Task<ApiResponse<object>?> CreateBulkSlotsAsync(
    CreateBulkSlotsViewModel model)
    {
        var response = await _http.PostAsJsonAsync(
            $"owner/areas/{model.ParkingAreaId}/slots/bulk",
            new
            {
                model.VehicleTypeId,
                model.Quantity,
                model.Prefix
            });

        return await response.Content
            .ReadFromJsonAsync<ApiResponse<object>>();
    }
}