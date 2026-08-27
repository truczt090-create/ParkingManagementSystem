using System;
using System.Collections.Generic;

namespace ParkingManagement.API.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? BookingId { get; set; }

    public int? ParkingSessionId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentType { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime PaidAt { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<BookingExtension> BookingExtensions { get; set; } = new List<BookingExtension>();

    public virtual ParkingSession? ParkingSession { get; set; }
}
