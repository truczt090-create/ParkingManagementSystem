using System;
using System.Collections.Generic;

namespace ParkingManagement.API.Models;

public partial class Owner
{
    public int OwnerId { get; set; }

    public int UserId { get; set; }

    public string? BusinessName { get; set; }

    public string? TaxCode { get; set; }

    public virtual ICollection<ParkingLot> ParkingLots { get; set; } = new List<ParkingLot>();

    public virtual User User { get; set; } = null!;
}
