namespace ParkingManagement.API.DTOs
{
    public record ParkingLotSummaryResponse(
         int ParkingLotId, string Name, string Address, string Status, string OwnerName);

    public record UserSummaryResponse(
        int UserId, string FullName, string Email, string Phone, string RoleName, bool IsActive, DateTime CreatedAt);
}
