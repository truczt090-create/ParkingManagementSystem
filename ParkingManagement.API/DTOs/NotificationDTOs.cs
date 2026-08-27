namespace ParkingManagement.API.DTOs
{
    public record NotificationResponse(
        int NotificationId, string Title, string Content, string Type, bool IsRead, DateTime CreatedAt);
}