namespace ParkingManagement.API.DTOs
{
    public record CreatePriceRequest(int ParkingLotId, int VehicleTypeId, string PriceType, decimal UnitPrice);
    public record UpdatePriceRequest(decimal UnitPrice);
    public record PriceResponse(int PriceId, int ParkingLotId, int VehicleTypeId, string VehicleTypeName, string PriceType, decimal UnitPrice);
}