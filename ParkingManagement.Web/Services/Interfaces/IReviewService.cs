using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.ViewModels.Booking;

namespace ParkingManagement.Web.Services.Interfaces;

public interface IReviewService
{
    Task<ApiResponse<ReviewsViewModel>?> GetReviewsAsync(int lotId, int page = 1);
    Task<ApiResponse<object>?> CreateReviewAsync(CreateReviewViewModel model);
}