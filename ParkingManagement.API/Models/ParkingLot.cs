using System;
using System.Collections.Generic;

namespace ParkingManagement.API.Models;

public partial class ParkingLot
{
    public int ParkingLotId { get; set; }

    public int OwnerId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual Owner Owner { get; set; } = null!;

    public virtual ICollection<ParkingArea> ParkingAreas { get; set; } = new List<ParkingArea>();

    public virtual ICollection<Price> Prices { get; set; } = new List<Price>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
