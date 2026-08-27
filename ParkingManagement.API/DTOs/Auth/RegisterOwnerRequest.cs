namespace ParkingManagement.API.DTOs.Auth
{
    public class RegisterOwnerRequest
    {
        public string FullName { get; set; } = "";

        public string Email { get; set; } = "";

        public string Password { get; set; } = "";

        public string? Phone { get; set; }

        public string BusinessName { get; set; } = "";

        public string? TaxCode { get; set; }
    }
}