namespace ParkingManagement.API.DTOs
{
    public record CreateParkingLotRequest(
        string Name,
        string Address,
        decimal? Latitude,
        decimal? Longitude,
        TimeOnly? OpenTime,
        TimeOnly? CloseTime
    );

    public record UpdateParkingLotRequest(
        string Name,
        string Address,
        decimal? Latitude,
        decimal? Longitude,
        TimeOnly? OpenTime,
        TimeOnly? CloseTime
    );

    public record ParkingLotResponse(
        int ParkingLotId,
        string Name,
        string Address,
        decimal? Latitude,
        decimal? Longitude,
        TimeOnly? OpenTime,
        TimeOnly? CloseTime,
        string Status
    );
}