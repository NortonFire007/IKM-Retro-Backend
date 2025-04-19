using System.ComponentModel.DataAnnotations;

namespace IKM_Retro.DTOs.User;

public class ChangePassword
{
    [Required(ErrorMessage = "Current password is required")]
    public string CurrentPassword { get; set; } = string.Empty;


    [Required(ErrorMessage = "New password is required")]
    public string NewPassword { get; set; } = string.Empty;
}