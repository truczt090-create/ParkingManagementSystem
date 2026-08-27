namespace ParkingManagement.Web.ViewModels.Owner;

public class CreateEmployeeViewModel
{
    public int ParkingLotId { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string? Phone { get; set; }
    public string? Shift { get; set; }
}

public class EmployeeSummaryViewModel
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Shift { get; set; }
}