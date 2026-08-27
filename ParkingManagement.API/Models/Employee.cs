using System;
using System.Collections.Generic;

namespace ParkingManagement.API.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public int UserId { get; set; }

    public int ParkingLotId { get; set; }

    public string? Shift { get; set; }

    public virtual ParkingLot ParkingLot { get; set; } = null!;

    public virtual ICollection<ParkingSession> ParkingSessionEmployeeIdCheckInNavigations { get; set; } = new List<ParkingSession>();

    public virtual ICollection<ParkingSession> ParkingSessionEmployeeIdCheckOutNavigations { get; set; } = new List<ParkingSession>();

    public virtual User User { get; set; } = null!;
}
