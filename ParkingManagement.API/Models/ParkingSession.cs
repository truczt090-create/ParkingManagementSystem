using System;
using System.Collections.Generic;

namespace ParkingManagement.API.Models;

public partial class ParkingSession
{
    public int ParkingSessionId { get; set; }

    public int? BookingId { get; set; }

    public int ParkingSlotId { get; set; }

    public string LicensePlate { get; set; } = null!;

    public int VehicleTypeId { get; set; }

    public DateTime CheckInTime { get; set; }

    public DateTime? CheckOutTime { get; set; }

    public int EmployeeIdCheckIn { get; set; }

    public int? EmployeeIdCheckOut { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Employee EmployeeIdCheckInNavigation { get; set; } = null!;

    public virtual Employee? EmployeeIdCheckOutNavigation { get; set; }

    public virtual ParkingSlot ParkingSlot { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual VehicleType VehicleType { get; set; } = null!;
}
