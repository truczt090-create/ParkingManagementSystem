using ParkingManagement.Web.Models.Common;
using ParkingManagement.Web.Models.ParkingLot;
namespace ParkingManagement.Web.ViewModels.Booking
{


    public class ParkingLotDetailPageViewModel
    {
        public ParkingLotViewModel Lot { get; set; } = null!;
        public List<VehicleTypeViewModel> VehicleTypes { get; set; } = new();
        public List<ParkingLotImageViewModel> Images { get; set; } = new();
        public int? SelectedVehicleTypeId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public AvailabilityViewModel? Availability { get; set; }
        public string? AvailabilityMessage { get; set; }
        public QuickStatsViewModel? QuickStats { get; set; }
        public ReviewsViewModel Reviews { get; set; } = new();
        public List<AmenityViewModel> Amenities { get; set; } = new();
       

    }
    public class AmenityViewModel { public int AmenityId { get; set; } public string Content { get; set; } = ""; }
    public class AvailabilityViewModel
    {
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public decimal? EstimatedAmount { get; set; }
    }

    public class CreateBookingViewModel
    {
        public int ParkingLotId { get; set; }
        public int VehicleTypeId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string LicensePlate { get; set; } = "";
    }

    public class BookingCreatedViewModel
    {
        public int BookingId { get; set; }
        public decimal TotalAmount { get; set; }
        public string LicensePlate { get; set; } = "";
    }

    public class PayBookingViewModel
    {
        public int BookingId { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = "ViDienTu";
    }

    public class MyBookingViewModel
    {
        public int BookingId { get; set; }
        public int ParkingLotId { get; set; }
        public string ParkingLotName { get; set; } = "";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = "";
        public bool IsPrepaid { get; set; }
        public decimal TotalAmount { get; set; }
    }
    public class ExtendBookingViewModel
    {
        public int BookingId { get; set; }
        public decimal ExtendedHours { get; set; }
    }

    public class ExtendResultViewModel
    {
        public DateTime NewEndTime { get; set; }
        public decimal AdditionalAmount { get; set; }
    }
    public class QuickStatsViewModel
    {
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ParkingLotImageViewModel
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; } = "";
    }


}
