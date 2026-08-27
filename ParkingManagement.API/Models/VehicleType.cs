using System;
using System.Collections.Generic;

namespace ParkingManagement.API.Models;

public partial class VehicleType
{
    public int VehicleTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<ParkingSession> ParkingSessions { get; set; } = new List<ParkingSession>();

    public virtual ICollection<ParkingSlot> ParkingSlots { get; set; } = new List<ParkingSlot>();

    public virtual ICollection<Price> Prices { get; set; } = new List<Price>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
