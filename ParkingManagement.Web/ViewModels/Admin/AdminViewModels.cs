namespace ParkingManagement.Web.ViewModels.Admin;
public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalParkingLots { get; set; }
    public int PendingParkingLots { get; set; }
    public decimal TotalRevenueAllLots { get; set; }
}

public class PendingLotViewModel
{
    public int ParkingLotId { get; set; }
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public string Status { get; set; } = "";
    public string OwnerName { get; set; } = "";
}

public class AdminUserViewModel
{
    public int UserId { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string RoleName { get; set; } = "";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}