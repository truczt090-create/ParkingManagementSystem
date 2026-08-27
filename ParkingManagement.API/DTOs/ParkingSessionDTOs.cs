namespace ParkingManagement.API.DTOs
{
    public record AvailableSlotResponse(int ParkingSlotId, string SlotCode);

    public record CheckInWithBookingRequest(int BookingId, string LicensePlate, int ParkingSlotId);

    public record CheckInWalkinRequest(string LicensePlate, int VehicleTypeId, int ParkingSlotId);

    public record CheckInResponse(int ParkingSessionId, string SlotCode, DateTime CheckInTime);

    public record CheckoutResponse(
        int ParkingSessionId,
        string CustomerType,       // "Booking" hoặc "VangLai"
        bool IsOvertime,
        decimal? OvertimeHours,
        decimal AmountToCollect,
        string PaymentMethod
    );
}