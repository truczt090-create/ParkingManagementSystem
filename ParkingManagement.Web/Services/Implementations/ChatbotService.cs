using System.Net.Http.Json;
using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.Services.Interfaces;
using ParkingManagement.Web.ViewModels.Chatbot;

namespace ParkingManagement.Web.Services.Implementations;

public class ChatbotService : IChatbotService
{
    private readonly HttpClient _http;
    public ChatbotService(IHttpClientFactory factory) => _http = factory.CreateClient("ParkingAPI");

    public async Task<ApiResponse<Dictionary<string, string>>?> AskAsync(AskRequestViewModel model)
    {
        var response = await _http.PostAsJsonAsync("chatbot/ask", model);
        return await response.Content.ReadFromJsonAsync<ApiResponse<Dictionary<string, string>>>();
    }
}