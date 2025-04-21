using System.ComponentModel.DataAnnotations;

namespace IKM_Retro.DTOs.User
{
    public class PostUserBody : UserProfileBase
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}

