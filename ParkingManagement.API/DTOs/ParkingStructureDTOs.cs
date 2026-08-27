namespace ParkingManagement.API.DTOs
{
    public record CreateAreaRequest(string Name);
    public record AreaResponse(int ParkingAreaId, string Name);

    public record CreateSlotRequest(int VehicleTypeId, string SlotCode);
    public record SlotResponse(int ParkingSlotId, string SlotCode, string Status, int VehicleTypeId);
    public record QuickStatsResponse(int TotalSlots, int AvailableSlots, decimal? MinPrice, decimal? MaxPrice, DateTime UpdatedAt);
    public record CreateBulkSlotsRequest(int VehicleTypeId, int Quantity,string Prefix);
}