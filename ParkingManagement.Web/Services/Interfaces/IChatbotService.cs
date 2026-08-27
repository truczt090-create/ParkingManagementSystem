using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.ViewModels.Chatbot;

namespace ParkingManagement.Web.Services.Interfaces;

public interface IChatbotService
{
    Task<ApiResponse<Dictionary<string, string>>?> AskAsync(AskRequestViewModel model);
}