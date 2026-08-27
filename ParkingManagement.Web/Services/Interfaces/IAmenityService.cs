using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.ViewModels.Booking;

namespace ParkingManagement.Web.Services.Interfaces
{
    public interface IAmenityService
    {
        Task<ApiResponse<List<AmenityViewModel>>?> GetAmenitiesAsync(int lotId);
    }
}
