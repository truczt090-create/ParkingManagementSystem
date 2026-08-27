using System;
using System.Collections.Generic;

namespace ParkingManagement.API.Models;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    public int UserId { get; set; }

    public int VehicleTypeId { get; set; }

    public string LicensePlate { get; set; } = null!;

    public string? Nickname { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual VehicleType VehicleType { get; set; } = null!;
}
