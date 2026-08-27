// Implementation
using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Booking;

public class AmenityService : IAmenityService
{
    private readonly HttpClient _http;
    public AmenityService(IHttpClientFactory factory) => _http = factory.CreateClient("ParkingAPI");

    public async Task<ApiResponse<List<AmenityViewModel>>?> GetAmenitiesAsync(int lotId)
    {
        var response = await _http.GetAsync($"parkinglots/{lotId}/amenities");
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<AmenityViewModel>>>();
    }
}