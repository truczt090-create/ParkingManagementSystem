using Microsoft.EntityFrameworkCore;
using ParkingManagement.API.Data;
using ParkingManagement.API.DTOs.Auth;
using ParkingManagement.API.Helpers;
using ParkingManagement.API.Models;
using ParkingManagement.API.Services.Interfaces;

namespace ParkingManagement.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly ParkingDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(ParkingDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users
     .Include(u => u.Role)
     .Include(u => u.Owner)
     .Include(u => u.Employee)
     .FirstOrDefaultAsync(
         u => u.Email == request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new Exception("Email hoặc mật khẩu không đúng");
        }
        //chặn tài khoản đăng nhập nếu tài khoản admin bị khóa
        if (!user.IsActive)
        {
            throw new Exception("Tài khoản đã bị khóa");
        }

        var token = JwtHelper.GenerateToken(user, _config);

        return new AuthResponse(
            token,
            user.FullName,
            user.Role.RoleName,
            user.AvatarUrl,
            user.Phone
        );
    }

    public async Task<int> RegisterAsync(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
        {
            throw new Exception("Email đã được sử dụng");
        }

        var customerRole = await _db.Roles
            .FirstAsync(r => r.RoleName == "Customer");

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = customerRole.RoleId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return user.UserId;
    }
    public async Task<int> RegisterOwnerAsync(RegisterOwnerRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
        {
            throw new Exception("Email đã được sử dụng");
        }

        var ownerRole = await _db.Roles
            .FirstAsync(r => r.RoleName == "Owner");

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = ownerRole.RoleId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var owner = new Owner
        {
            UserId = user.UserId,
            BusinessName = request.BusinessName,
            TaxCode = request.TaxCode
        };

        _db.Owners.Add(owner);
        await _db.SaveChangesAsync();

        return user.UserId;
    }
}