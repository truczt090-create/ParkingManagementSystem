using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.ViewModels.Notification;

namespace ParkingManagement.Web.Services.Interfaces;

public interface INotificationService
{
    Task<ApiResponse<List<NotificationViewModel>>?> GetMyNotificationsAsync();
    Task MarkAsReadAsync(int id);
}