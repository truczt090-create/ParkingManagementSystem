using System.Net.Http.Json;
using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Notification;

namespace ParkingManagement.Web.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly HttpClient _http;
    public NotificationService(IHttpClientFactory factory) => _http = factory.CreateClient("ParkingAPI");

    public async Task<ApiResponse<List<NotificationViewModel>>?> GetMyNotificationsAsync()
    {
        var response = await _http.GetAsync("notifications");
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<NotificationViewModel>>>();
    }

    public async Task MarkAsReadAsync(int id)
    {
        await _http.PostAsync($"notifications/{id}/read", null);
    }
}