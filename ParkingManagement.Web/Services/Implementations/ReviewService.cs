using System.Net.Http.Json;
using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Booking;

namespace ParkingManagement.Web.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly HttpClient _http;
    public ReviewService(IHttpClientFactory factory) => _http = factory.CreateClient("ParkingAPI");

    public async Task<ApiResponse<ReviewsViewModel>?> GetReviewsAsync(int lotId, int page = 1)
    {
        var response = await _http.GetAsync($"parkinglots/{lotId}/reviews?page={page}&pageSize=6");
        return await response.Content.ReadFromJsonAsync<ApiResponse<ReviewsViewModel>>();
    }

    public async Task<ApiResponse<object>?> CreateReviewAsync(CreateReviewViewModel model)
    {
        var response = await _http.PostAsJsonAsync($"parkinglots/{model.ParkingLotId}/reviews",
            new { model.Rating, model.Comment });
        return await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
    }
}