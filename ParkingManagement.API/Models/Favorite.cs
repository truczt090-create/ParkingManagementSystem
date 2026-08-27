using System;
using System.Collections.Generic;

namespace ParkingManagement.API.Models;

public partial class Favorite
{
    public int FavoriteId { get; set; }

    public int UserId { get; set; }

    public int ParkingLotId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ParkingLot ParkingLot { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
