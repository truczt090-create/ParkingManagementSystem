namespace ParkingManagement.Web.ViewModels.Employee;

public class VehicleDashboardViewModel
{
    public string ParkingLotName { get; set; } = "";

    public int TotalSlots { get; set; }

    public int AvailableSlots { get; set; }

    public int OccupiedSlots { get; set; }

    public int ReservedSlots { get; set; }

    public List<ParkingSlotDashboardViewModel> Slots { get; set; } = new();
}

public class ParkingSlotDashboardViewModel
{
    public int ParkingSlotId { get; set; }

    public string SlotCode { get; set; } = "";

    public string Status { get; set; } = "";

    public string? LicensePlate { get; set; }

    public int? BookingId { get; set; }

    public DateTime? CheckInTime { get; set; }
    public int? ParkingSessionId { get; set; }
}