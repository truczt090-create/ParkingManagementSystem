using System;
using System.Collections.Generic;

namespace ParkingManagement.API.Models;

public partial class ParkingSlot
{
    public int ParkingSlotId { get; set; }

    public int ParkingAreaId { get; set; }

    public int VehicleTypeId { get; set; }

    public string SlotCode { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ParkingArea ParkingArea { get; set; } = null!;

    public virtual ICollection<ParkingSession> ParkingSessions { get; set; } = new List<ParkingSession>();

    public virtual VehicleType VehicleType { get; set; } = null!;
}
