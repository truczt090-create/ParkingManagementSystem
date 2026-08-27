using System;
using System.Collections.Generic;

namespace ParkingManagement.API.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int UserId { get; set; }

    public int ParkingLotId { get; set; }

    public byte Rating { get; set; }

    public string? Comment { get; set; }

    public string? OwnerReply { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ParkingLot ParkingLot { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
