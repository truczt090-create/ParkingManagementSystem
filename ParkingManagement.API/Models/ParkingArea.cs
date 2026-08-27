using System;
using System.Collections.Generic;

namespace ParkingManagement.API.Models;

public partial class ParkingArea
{
    public int ParkingAreaId { get; set; }

    public int ParkingLotId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ParkingLot ParkingLot { get; set; } = null!;

    public virtual ICollection<ParkingSlot> ParkingSlots { get; set; } = new List<ParkingSlot>();
}
