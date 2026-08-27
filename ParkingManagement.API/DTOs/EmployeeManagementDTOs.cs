namespace ParkingManagement.API.DTOs
{
    public record CreateEmployeeRequest(string FullName, string Email, string Password, string? Phone, string? Shift);
    public record EmployeeSummaryResponse(int EmployeeId, string FullName, string Email, string? Shift);
}
