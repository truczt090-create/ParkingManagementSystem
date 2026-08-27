using System;
using System.Collections.Generic;

namespace ParkingManagement.API.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int ParkingLotId { get; set; }

    public int VehicleTypeId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string Status { get; set; } = null!;

    public bool IsPrepaid { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? LicensePlate { get; set; }

    public virtual ICollection<BookingExtension> BookingExtensions { get; set; } = new List<BookingExtension>();

    public virtual ParkingLot ParkingLot { get; set; } = null!;

    public virtual ICollection<ParkingSession> ParkingSessions { get; set; } = new List<ParkingSession>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;

    public virtual VehicleType VehicleType { get; set; } = null!;
}
