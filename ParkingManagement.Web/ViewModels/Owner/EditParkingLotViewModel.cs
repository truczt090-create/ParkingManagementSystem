using System.ComponentModel.DataAnnotations;

namespace ParkingManagement.Web.ViewModels.Owner;

public class EditParkingLotViewModel
{
    public int ParkingLotId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên bãi xe")]
    [Display(Name = "Tên bãi xe")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
    [Display(Name = "Địa chỉ")]
    public string Address { get; set; } = "";

    [Display(Name = "Vĩ độ")]
    public decimal? Latitude { get; set; }

    [Display(Name = "Kinh độ")]
    public decimal? Longitude { get; set; }

    [Display(Name = "Giờ mở cửa")]
    public TimeOnly? OpenTime { get; set; }

    [Display(Name = "Giờ đóng cửa")]
    public TimeOnly? CloseTime { get; set; }

    public string Status { get; set; } = "";
}