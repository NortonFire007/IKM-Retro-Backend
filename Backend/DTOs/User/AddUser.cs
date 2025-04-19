using System.ComponentModel.DataAnnotations;

namespace IKM_Retro.DTOs.User
{
    public class AddUser : UserProfileBase
    {
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}

