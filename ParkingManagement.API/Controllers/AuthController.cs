using Microsoft.AspNetCore.Mvc;   
using ParkingManagement.API.DTOs.Auth;
using ParkingManagement.API.Services.Interfaces;
using ParkingManagement.API.Helpers;
using ParkingManagement.API.Models;

namespace ParkingManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            try
            {
                var userId = await _authService.RegisterAsync(req);

                return Created("", ApiResponse<object>.Ok(
                    new { UserId = userId },
                    "Đăng ký thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            try
            {
                var result = await _authService.LoginAsync(req);

                return Ok(ApiResponse<AuthResponse>.Ok(
                    result,
                    "Đăng nhập thành công"));
            }
            catch (Exception ex)
            {
                return Unauthorized(ApiResponse<object>.Fail(ex.Message));
            }
        }
        [HttpPost("register-owner")]
        public async Task<IActionResult> RegisterOwner(RegisterOwnerRequest req)
        {
            try
            {
                var userId = await _authService.RegisterOwnerAsync(req);

                return Created("", ApiResponse<object>.Ok(
                    new { UserId = userId },
                    "Đăng ký chủ bãi xe thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
    }
}