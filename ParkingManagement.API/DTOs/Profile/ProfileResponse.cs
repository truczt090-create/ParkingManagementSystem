namespace ParkingManagement.API.DTOs.Profile;

public class ProfileResponse
{
    // =========================
    // THÔNG TIN CHUNG
    // =========================

    public int UserId { get; set; }

    public string FullName { get; set; } = "";

    public string Email { get; set; } = "";

    public string? Phone { get; set; }

    public string? AvatarUrl { get; set; }

    public string Role { get; set; } = "";

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }


    // =========================
    // CUSTOMER
    // =========================

    public int BookingCount { get; set; }

    public int UnreadNotificationCount { get; set; }

    public List<ProfileVehicleResponse> Vehicles { get; set; } = new();


    // =========================
    // OWNER
    // =========================

    public string? BusinessName { get; set; }

    public string? TaxCode { get; set; }

    public int ParkingLotCount { get; set; }

    public int EmployeeCount { get; set; }
}


public class ProfileVehicleResponse
{
    public int VehicleId { get; set; }

    public string LicensePlate { get; set; } = "";

    public string VehicleType { get; set; } = "";

    public string? Nickname { get; set; }
}