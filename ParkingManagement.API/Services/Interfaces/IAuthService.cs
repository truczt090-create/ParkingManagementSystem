using ParkingManagement.API.DTOs.Auth;

namespace ParkingManagement.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);

    Task<int> RegisterAsync(RegisterRequest request);
    Task<int> RegisterOwnerAsync(RegisterOwnerRequest request);
}