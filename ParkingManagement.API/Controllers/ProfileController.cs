using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingManagement.API.DTOs.Profile;
using ParkingManagement.API.Helpers;
using ParkingManagement.API.Services.Interfaces;

namespace ParkingManagement.API.Controllers;

[ApiController]
[Route("api/v1/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    private int GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userIdValue) ||
            !int.TryParse(userIdValue, out var userId))
        {
            throw new UnauthorizedAccessException(
                "Không xác định được người dùng đăng nhập");
        }

        return userId;
    }


    // ============================================
    // GET: api/v1/profile/me
    // ============================================

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var userId = GetCurrentUserId();

            var result =
                await _profileService.GetProfileAsync(userId);

            return Ok(
                ApiResponse<ProfileResponse>.Ok(
                    result,
                    "Lấy thông tin hồ sơ thành công"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<object>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<object>.Fail(ex.Message));
        }
    }


    // ============================================
    // PUT: api/v1/profile/me
    // ============================================

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile(
        UpdateProfileRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();

            var result =
                await _profileService.UpdateProfileAsync(
                    userId,
                    request);

            return Ok(
                ApiResponse<ProfileResponse>.Ok(
                    result,
                    "Cập nhật hồ sơ thành công"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<object>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<object>.Fail(ex.Message));
        }
    }


    // ============================================
    // POST: api/v1/profile/change-password
    // ============================================

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();

            await _profileService.ChangePasswordAsync(
                userId,
                request);

            return Ok(
                ApiResponse<object>.Ok(
                    null,
                    "Đổi mật khẩu thành công"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<object>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<object>.Fail(ex.Message));
        }
    }
}