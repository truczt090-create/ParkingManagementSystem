namespace ParkingManagement.Web.ViewModels.Owner;

public class RevenuePointViewModel { public string Period { get; set; } = ""; public decimal Amount { get; set; } }

public class RevenueViewModel
{
    public decimal TotalRevenue { get; set; }
    public List<RevenuePointViewModel> Breakdown { get; set; } = new();
}

public class OccupancyViewModel
{
    public int TotalSlots { get; set; }
    public int OccupiedSlots { get; set; }
    public double OccupancyRate { get; set; }
}

public class OwnerLotViewModel
{
    public int ParkingLotId { get; set; }
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public string Status { get; set; } = "";
}

public class AreaViewModel 
{   public int ParkingAreaId { get; set; }
    public string Name { get; set; } = "";
}

public class SlotViewModel
{
    public int ParkingSlotId { get; set; }
    public string SlotCode { get; set; } = "";
    public string Status { get; set; } = "";
    public int VehicleTypeId { get; set; }
}

public class PriceViewModel
{
    public int PriceId { get; set; }
    public int ParkingLotId { get; set; }
    public int VehicleTypeId { get; set; }
    public string VehicleTypeName { get; set; } = "";
    public string PriceType { get; set; } = "";
    public decimal UnitPrice { get; set; }
}

public class CreateAreaViewModel { public int ParkingLotId { get; set; } public string Name { get; set; } = ""; }

public class CreateSlotViewModel
{
    public int ParkingAreaId { get; set; }
    public int VehicleTypeId { get; set; }
    public string SlotCode { get; set; } = "";
}

public class CreatePriceViewModel
{
    public int ParkingLotId { get; set; }
    public int VehicleTypeId { get; set; }
    public string PriceType { get; set; } = "Booking";
    public decimal UnitPrice { get; set; }
}
public class OwnerDashboardSummaryViewModel
{
    public int ParkingLotCount { get; set; }
    public int AreaCount { get; set; }
    public int TotalSlots { get; set; }
    public int EmployeeCount { get; set; }
    public double AverageRating { get; set; }
    public int BookingCount { get; set; }
}
public class CreateBulkSlotsViewModel
{
    public int ParkingAreaId { get; set; }

    public int VehicleTypeId { get; set; }

    public int Quantity { get; set; }

    public string Prefix { get; set; } = "A";
}
