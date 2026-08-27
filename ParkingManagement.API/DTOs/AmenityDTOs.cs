namespace ParkingManagement.API.DTOs
{
    public record AmenityResponse(int AmenityId, string Content);
    public record CreateAmenityRequest(string Content);
}