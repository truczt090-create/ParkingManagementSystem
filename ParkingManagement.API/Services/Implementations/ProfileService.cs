using Microsoft.EntityFrameworkCore;
using ParkingManagement.API.Data;
using ParkingManagement.API.DTOs.Profile;
using ParkingManagement.API.Services.Interfaces;

namespace ParkingManagement.API.Services.Implementations;

public class ProfileService : IProfileService
{
    private readonly ParkingDbContext _db;

    public ProfileService(ParkingDbContext db)
    {
        _db = db;
    }


    // =================================================
    // LẤY HỒ SƠ
    // =================================================

    public async Task<ProfileResponse> GetProfileAsync(int userId)
    {
        var user = await _db.Users
            .Include(u => u.Role)
            .Include(u => u.Owner)
            .Include(u => u.Vehicles)
                .ThenInclude(v => v.VehicleType)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            throw new Exception("Không tìm thấy người dùng");
        }


        var result = new ProfileResponse
        {
            UserId = user.UserId,

            FullName = user.FullName,

            Email = user.Email,

            Phone = user.Phone,

            AvatarUrl = user.AvatarUrl,

            Role = user.Role.RoleName,

            CreatedAt = user.CreatedAt,

            IsActive = user.IsActive
        };


        // ============================================
        // CUSTOMER
        // ============================================

        if (user.Role.RoleName == "Customer")
        {
            result.BookingCount = await _db.Bookings
                .CountAsync(b => b.UserId == userId);

            result.UnreadNotificationCount =
                await _db.Notifications
                    .CountAsync(n =>
                        n.UserId == userId &&
                        !n.IsRead);

            result.Vehicles = user.Vehicles
                .Select(v => new ProfileVehicleResponse
                {
                    VehicleId = v.VehicleId,

                    LicensePlate = v.LicensePlate,

                    VehicleType = v.VehicleType.TypeName,

                    Nickname = v.Nickname
                })
                .ToList();
        }


        // ============================================
        // OWNER
        // ============================================

        if (user.Role.RoleName == "Owner")
        {
            var owner = user.Owner;

            if (owner != null)
            {
                result.BusinessName = owner.BusinessName;

                result.TaxCode = owner.TaxCode;

                result.ParkingLotCount =
                    await _db.ParkingLots
                        .CountAsync(p =>
                            p.OwnerId == owner.OwnerId);

                result.EmployeeCount =
                    await _db.Employees
                        .CountAsync(e =>
                            _db.ParkingLots.Any(p =>
                                p.ParkingLotId ==
                                e.ParkingLotId &&
                                p.OwnerId ==
                                owner.OwnerId));
            }
        }


        return result;
    }


    // =================================================
    // CẬP NHẬT HỒ SƠ
    // =================================================

    public async Task<ProfileResponse> UpdateProfileAsync(
        int userId,
        UpdateProfileRequest request)
    {
        var user = await _db.Users
            .Include(u => u.Role)
            .Include(u => u.Owner)
            .FirstOrDefaultAsync(u =>
                u.UserId == userId);

        if (user == null)
        {
            throw new Exception(
                "Không tìm thấy người dùng");
        }


        // =========================
        // UPDATE USER
        // =========================

        user.FullName = request.FullName.Trim();

        user.Phone = string.IsNullOrWhiteSpace(
            request.Phone)
                ? null
                : request.Phone.Trim();


        // =========================
        // UPDATE OWNER
        // =========================

        if (user.Role.RoleName == "Owner")
        {
            if (user.Owner == null)
            {
                throw new Exception(
                    "Không tìm thấy thông tin chủ bãi");
            }

            user.Owner.BusinessName =
                string.IsNullOrWhiteSpace(
                    request.BusinessName)
                    ? null
                    : request.BusinessName.Trim();

            user.Owner.TaxCode =
                string.IsNullOrWhiteSpace(
                    request.TaxCode)
                    ? null
                    : request.TaxCode.Trim();
        }


        await _db.SaveChangesAsync();


        return await GetProfileAsync(userId);
    }


    // =================================================
    // ĐỔI MẬT KHẨU
    // =================================================

    public async Task ChangePasswordAsync(
        int userId,
        ChangePasswordRequest request)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u =>
                u.UserId == userId);

        if (user == null)
        {
            throw new Exception(
                "Không tìm thấy người dùng");
        }


        // Kiểm tra mật khẩu cũ
        var passwordCorrect =
            BCrypt.Net.BCrypt.Verify(
                request.CurrentPassword,
                user.PasswordHash);

        if (!passwordCorrect)
        {
            throw new Exception(
                "Mật khẩu hiện tại không đúng");
        }


        // Kiểm tra mật khẩu mới
        if (string.IsNullOrWhiteSpace(
            request.NewPassword))
        {
            throw new Exception(
                "Mật khẩu mới không được để trống");
        }


        if (request.NewPassword.Length < 6)
        {
            throw new Exception(
                "Mật khẩu mới phải có ít nhất 6 ký tự");
        }


        if (request.NewPassword !=
            request.ConfirmPassword)
        {
            throw new Exception(
                "Xác nhận mật khẩu không khớp");
        }


        // Không cho đổi thành mật khẩu cũ
        if (BCrypt.Net.BCrypt.Verify(
            request.NewPassword,
            user.PasswordHash))
        {
            throw new Exception(
                "Mật khẩu mới phải khác mật khẩu hiện tại");
        }


        user.PasswordHash =
            BCrypt.Net.BCrypt.HashPassword(
                request.NewPassword);


        await _db.SaveChangesAsync();
    }
}