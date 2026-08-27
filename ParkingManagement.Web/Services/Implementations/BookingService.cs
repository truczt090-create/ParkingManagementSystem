using System.Net.Http.Json;
using ParkingManagement.Web.ViewModels.Booking;
using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.Services.Interfaces;

namespace ParkingManagement.Web.Services.Implementations;

public class BookingService : IBookingService
{
    private readonly HttpClient _http;

    public BookingService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("ParkingAPI");
    }

    public async Task<ApiResponse<AvailabilityViewModel>?> CheckAvailabilityAsync(
        int lotId, int vehicleTypeId, DateTime startTime, DateTime endTime)
    {
        var url = $"parkinglots/{lotId}/availability?vehicleTypeId={vehicleTypeId}" +
                  $"&startTime={startTime:o}&endTime={endTime:o}";
        var response = await _http.GetAsync(url);
        return await response.Content.ReadFromJsonAsync<ApiResponse<AvailabilityViewModel>>();
    }

    public async Task<ApiResponse<BookingCreatedViewModel>?> CreateBookingAsync(CreateBookingViewModel model)
    {
        var response = await _http.PostAsJsonAsync("bookings", model);
        return await response.Content.ReadFromJsonAsync<ApiResponse<BookingCreatedViewModel>>();
    }

    public async Task<ApiResponse<object>?> PayBookingAsync(int bookingId, string paymentMethod)
    {
        var response = await _http.PostAsJsonAsync($"bookings/{bookingId}/pay", new { paymentMethod });
        return await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
    }

    public async Task<ApiResponse<List<MyBookingViewModel>>?> GetMyBookingsAsync()
    {
        var response = await _http.GetAsync("bookings/my");
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<MyBookingViewModel>>>();
    }

    public async Task<ApiResponse<object>?> CancelBookingAsync(int bookingId)
    {
        var response = await _http.DeleteAsync($"bookings/{bookingId}");
        return await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
    }
    public async Task<ApiResponse<ExtendResultViewModel>?> ExtendBookingAsync(ExtendBookingViewModel model)
    {
        var response = await _http.PostAsJsonAsync($"bookings/{model.BookingId}/extend",
            new { ExtendedHours = model.ExtendedHours });
        return await response.Content.ReadFromJsonAsync<ApiResponse<ExtendResultViewModel>>();
    }
}
