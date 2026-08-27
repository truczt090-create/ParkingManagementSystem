namespace ParkingManagement.Web.Models.ParkingLot;

public class ParkingLotViewModel
{
    public int ParkingLotId { get; set; }
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public TimeOnly? OpenTime { get; set; }
    public TimeOnly? CloseTime { get; set; }
    public string Status { get; set; } = "";
    public string? Description { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

}