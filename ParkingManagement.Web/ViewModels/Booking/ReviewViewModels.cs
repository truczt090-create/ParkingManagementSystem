namespace ParkingManagement.Web.ViewModels.Booking;

public class ReviewItemViewModel
{
    public int ReviewId { get; set; }
    public string UserName { get; set; } = "";
    public string? AvatarUrl { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string? OwnerReply { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ReviewsViewModel
{
    public double AverageRating { get; set; }
    public int TotalCount { get; set; }
    public Dictionary<int, int> RatingBreakdown { get; set; } = new();
    public List<ReviewItemViewModel> Items { get; set; } = new();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 6;
    public int TotalPages { get; set; }
}

public class CreateReviewViewModel
{
    public int ParkingLotId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
