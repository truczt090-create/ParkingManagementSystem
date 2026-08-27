namespace ParkingManagement.Web.ViewModels.Notification;

public class NotificationViewModel
{
    public int NotificationId { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Type { get; set; } = "";
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}