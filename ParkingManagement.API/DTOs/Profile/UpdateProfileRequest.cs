namespace ParkingManagement.API.DTOs.Profile;

public class UpdateProfileRequest
{
    public string FullName { get; set; } = "";

    public string? Phone { get; set; }

    public string? BusinessName { get; set; }

    public string? TaxCode { get; set; }
}