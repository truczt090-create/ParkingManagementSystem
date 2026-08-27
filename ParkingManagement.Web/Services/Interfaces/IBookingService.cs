
using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.ViewModels.Booking;

namespace ParkingManagement.Web.Services.Interfaces;

public interface IBookingService
{
    Task<ApiResponse<AvailabilityViewModel>?> CheckAvailabilityAsync(int lotId, int vehicleTypeId, DateTime startTime, DateTime endTime);
    Task<ApiResponse<BookingCreatedViewModel>?> CreateBookingAsync(CreateBookingViewModel model);
    Task<ApiResponse<object>?> PayBookingAsync(int bookingId, string paymentMethod);
    Task<ApiResponse<List<MyBookingViewModel>>?> GetMyBookingsAsync();
    Task<ApiResponse<object>?> CancelBookingAsync(int bookingId);
    Task<ApiResponse<ExtendResultViewModel>?> ExtendBookingAsync(ExtendBookingViewModel model);
}