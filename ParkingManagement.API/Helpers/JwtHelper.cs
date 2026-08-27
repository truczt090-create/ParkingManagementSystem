using Microsoft.IdentityModel.Tokens;
using ParkingManagement.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ParkingManagement.API.Helpers
{
    public static class JwtHelper
    {
        public static string GenerateToken(User user, IConfiguration config)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Nếu là Employee, thêm ParkingLotId
            if (user.Role.RoleName == "Employee" && user.Employee != null)
            {
                claims.Add(new Claim(
                    "ParkingLotId",
                    user.Employee.ParkingLotId.ToString()
                ));
            }

            // Nếu là Owner, thêm OwnerId
            if (user.Role.RoleName == "Owner" && user.Owner != null)
            {
                claims.Add(new Claim(
                    "OwnerId",
                    user.Owner.OwnerId.ToString()
                ));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(config["Jwt:ExpiryMinutes"]!)
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}