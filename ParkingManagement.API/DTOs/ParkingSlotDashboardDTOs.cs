namespace ParkingManagement.API.DTOs
{
    public class ParkingSlotDashboardDTOs
    {
        public int ParkingSlotId { get; set; }

        public string SlotCode { get; set; } = "";

        public string Status { get; set; } = "";

        public string? LicensePlate { get; set; }

        public int? BookingId { get; set; }

        public DateTime? CheckInTime { get; set; }
    }
}
