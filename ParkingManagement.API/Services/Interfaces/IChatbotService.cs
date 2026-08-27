using ParkingManagement.API.DTOs.Chatbot;

namespace ParkingManagement.API.Services.Interfaces
{
    public interface IChatbotService
    {
        Task<string> AskAsync(ChatRequest request);
    }
}