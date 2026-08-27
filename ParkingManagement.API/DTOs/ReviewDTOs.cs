namespace ParkingManagement.API.DTOs
{
    public record ReviewItemResponse(
        int ReviewId, string UserName, string? AvatarUrl, int Rating, string? Comment, string? OwnerReply, DateTime CreatedAt);

    public record ReviewsResponse(
        double AverageRating, int TotalCount, Dictionary<int, int> RatingBreakdown,
        List<ReviewItemResponse> Items, int Page, int PageSize, int TotalPages);

    public record CreateReviewRequest(int Rating, string? Comment);
}