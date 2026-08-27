using System.ComponentModel.DataAnnotations;

namespace ParkingManagement.Web.ViewModels.Profile;

public class ChangePasswordViewModel
{
    [Required(
        ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
    public string CurrentPassword { get; set; } = "";


    [Required(
        ErrorMessage = "Vui lòng nhập mật khẩu mới")]
    [MinLength(
        6,
        ErrorMessage =
            "Mật khẩu mới phải có ít nhất 6 ký tự")]
    public string NewPassword { get; set; } = "";


    [Required(
        ErrorMessage =
            "Vui lòng xác nhận mật khẩu mới")]
    [Compare(
        nameof(NewPassword),
        ErrorMessage =
            "Xác nhận mật khẩu không khớp")]
    public string ConfirmPassword { get; set; } = "";
}