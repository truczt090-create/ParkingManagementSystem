using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagement.API.Data;
using ParkingManagement.API.DTOs;
using ParkingManagement.API.Helpers;
using ParkingManagement.API.Models;

namespace ParkingManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/owner/employees")]
    [Authorize(Roles = "Owner")]
    public class OwnerEmployeesController : ControllerBase
    {
        private readonly ParkingDbContext _db;
        public OwnerEmployeesController(ParkingDbContext db) => _db = db;

        private int GetOwnerId() => int.Parse(User.FindFirst("OwnerId")!.Value);

        [HttpGet]
        public async Task<IActionResult> GetMyEmployees([FromQuery] int parkingLotId)
        {
            var ownerId = GetOwnerId();
            var lot = await _db.ParkingLots.FirstOrDefaultAsync(l => l.ParkingLotId == parkingLotId);
            if (lot == null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy bãi xe"));
            if (lot.OwnerId != ownerId) return Forbid();

            var employees = await _db.Employees
                .Include(e => e.User)
                .Where(e => e.ParkingLotId == parkingLotId)
                .Select(e => new EmployeeSummaryResponse(e.EmployeeId, e.User.FullName, e.User.Email, e.Shift))
                .ToListAsync();

            return Ok(ApiResponse<object>.Ok(employees));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromQuery] int parkingLotId, CreateEmployeeRequest req)
        {
            var ownerId = GetOwnerId();
            var lot = await _db.ParkingLots.FirstOrDefaultAsync(l => l.ParkingLotId == parkingLotId);
            if (lot == null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy bãi xe"));
            if (lot.OwnerId != ownerId) return Forbid();

            if (await _db.Users.AnyAsync(u => u.Email == req.Email))
                return BadRequest(ApiResponse<object>.Fail("Email đã được sử dụng"));

            var employeeRole = await _db.Roles.FirstAsync(r => r.RoleName == "Employee");

            var user = new User
            {
                FullName = req.FullName,
                Email = req.Email,
                Phone = req.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                RoleId = employeeRole.RoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var employee = new Employee
            {
                UserId = user.UserId,
                ParkingLotId = parkingLotId,
                Shift = req.Shift
            };
            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();

            return Created("", ApiResponse<object>.Ok(
                new { employee.EmployeeId }, "Đã tạo tài khoản nhân viên"));
        }

        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> RemoveEmployee(int employeeId)
        {
            var ownerId = GetOwnerId();
            var employee = await _db.Employees
                .Include(e => e.ParkingLot)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null) return NotFound(ApiResponse<object>.Fail("Không tìm thấy nhân viên"));
            if (employee.ParkingLot.OwnerId != ownerId) return Forbid();

            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok(null, "Đã xóa nhân viên (tài khoản đăng nhập vẫn còn, chỉ mất quyền Employee tại bãi này)"));
        }
    }
}