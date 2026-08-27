namespace ParkingManagement.Web.ViewModels.Employee;

public class AvailableSlotViewModel
{
    public int ParkingSlotId { get; set; }
    public string SlotCode { get; set; } = "";
}

public class ActiveSessionViewModel
{
    public int ParkingSessionId { get; set; }
    public string LicensePlate { get; set; } = "";
    public DateTime CheckInTime { get; set; }
    public string SlotCode { get; set; } = "";
}

public class CheckInWithBookingViewModel
{
    public int BookingId { get; set; }
    public string LicensePlate { get; set; } = "";
    public int ParkingSlotId { get; set; }
}

public class CheckInWalkinViewModel
{
    public string LicensePlate { get; set; } = "";
    public int VehicleTypeId { get; set; }
    public int ParkingSlotId { get; set; }
}

public class CheckInResultViewModel
{
    public int ParkingSessionId { get; set; }
    public string SlotCode { get; set; } = "";
    public DateTime CheckInTime { get; set; }
}

public class CheckoutResultViewModel
{
    public int ParkingSessionId { get; set; }
    public string CustomerType { get; set; } = "";   // "Booking" hoặc "VangLai"
    public bool IsOvertime { get; set; }
    public decimal? OvertimeHours { get; set; }
    public decimal AmountToCollect { get; set; }
    public string PaymentMethod { get; set; } = "";
}
