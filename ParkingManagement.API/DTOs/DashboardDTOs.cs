namespace ParkingManagement.API.DTOs
{
    public record RevenuePoint(string Period, decimal Amount);
    public record RevenueResponse(decimal TotalRevenue, List<RevenuePoint> Breakdown);

    public record OccupancyResponse(int TotalSlots, int OccupiedSlots, double OccupancyRate);

    public record AdminDashboardResponse(
        int TotalUsers,
        int TotalParkingLots,
        int PendingParkingLots,
        decimal TotalRevenueAllLots
    );
}