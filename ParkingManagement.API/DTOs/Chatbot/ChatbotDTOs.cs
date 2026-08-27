namespace ParkingManagement.API.DTOs.Chatbot
{
    public record ChatRequest(int? ParkingLotId, string Question);
    public record ChatResponse(string Answer);
}