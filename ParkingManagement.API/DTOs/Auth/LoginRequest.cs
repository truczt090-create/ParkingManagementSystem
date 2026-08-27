namespace ParkingManagement.API.DTOs.Auth;

public record LoginRequest(
    string Email,
    string Password
);