namespace ParkingManagement.API.Models
{
    public class ParkingLotAmenity
    {
        public int AmenityId { get; set; }
        public int ParkingLotId { get; set; }
        public string Content { get; set; } = "";
        public virtual ParkingLot ParkingLot { get; set; } = null!;
    }
}