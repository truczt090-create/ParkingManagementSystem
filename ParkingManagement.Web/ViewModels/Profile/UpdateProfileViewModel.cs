using System.ComponentModel.DataAnnotations;

namespace ParkingManagement.Web.ViewModels.Profile;

public class UpdateProfileViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    public string FullName { get; set; } = "";

    public string? Phone { get; set; }

    public string? BusinessName { get; set; }

    public string? TaxCode { get; set; }

    public string Role { get; set; } = "";

    public string Email { get; set; } = "";

    public string? AvatarUrl { get; set; }
}