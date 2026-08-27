namespace ParkingManagement.API.DTOs
{
    public class CreateParkingLotImageRequest
    {
        public string ImageUrl { get; set; } = "";
        public bool IsPrimary { get; set; }
    }
}
