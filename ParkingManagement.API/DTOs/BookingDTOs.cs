namespace ParkingManagement.API.DTOs
{
    public record CreateBookingRequest(
        int ParkingLotId,
        int VehicleTypeId,
        DateTime StartTime,
        DateTime EndTime,
        string LicensePlate
    );

    public record PayBookingRequest(string PaymentMethod); // "ViDienTu" hoặc "TheMoPhong"

    public record ExtendBookingRequest(decimal ExtendedHours);
    public record ExtendBookingResponse(DateTime NewEndTime, decimal AdditionalAmount);
    public record AvailabilityResponse(int TotalSlots, int AvailableSlots, decimal? EstimatedAmount);
    public record BookingResponse(
        int BookingId,
        int ParkingLotId,
        string ParkingLotName,
        DateTime StartTime,
        DateTime EndTime,
        string Status,
        bool IsPrepaid,
        decimal TotalAmount
    );
   
}
