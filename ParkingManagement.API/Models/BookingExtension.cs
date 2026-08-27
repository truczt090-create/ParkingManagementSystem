using System;
using System.Collections.Generic;

namespace ParkingManagement.API.Models;

public partial class BookingExtension
{
    public int ExtensionId { get; set; }

    public int BookingId { get; set; }

    public decimal ExtendedHours { get; set; }

    public decimal Amount { get; set; }

    public int? PaymentId { get; set; }

    public DateTime ExtendedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Payment? Payment { get; set; }
}
