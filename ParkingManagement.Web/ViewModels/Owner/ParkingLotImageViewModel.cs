namespace ParkingManagement.Web.ViewModels.Owner
{
    public class ParkingLotImageViewModel
    {
        public int ImageId { get; set; }

        public string ImageUrl { get; set; } = "";

        public bool IsPrimary { get; set; }
    }
}
