using System;
using System.Collections.Generic;

namespace ParkingManagement.API.Models;

public partial class Image
{
    public int ImageId { get; set; }

    public int ParkingLotId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public virtual ParkingLot ParkingLot { get; set; } = null!;
}
