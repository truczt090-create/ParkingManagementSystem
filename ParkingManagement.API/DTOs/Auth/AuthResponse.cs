namespace ParkingManagement.API.DTOs.Auth;

public record AuthResponse(
    string Token,
    string FullName,
    string Role,
    string? AvatarUrl,
    string? Phone
);
