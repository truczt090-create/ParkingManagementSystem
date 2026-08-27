using System;
using System.Collections.Generic;

namespace ParkingManagement.API.Models;

public partial class Price
{
    public int PriceId { get; set; }

    public int ParkingLotId { get; set; }

    public int VehicleTypeId { get; set; }

    public string PriceType { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public virtual ParkingLot ParkingLot { get; set; } = null!;

    public virtual VehicleType VehicleType { get; set; } = null!;
}
