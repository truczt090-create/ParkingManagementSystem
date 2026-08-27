namespace ParkingManagement.Web.ViewModels.Chatbot;

public class AskRequestViewModel
{
    public int? ParkingLotId { get; set; }
    public string Question { get; set; } = "";
}