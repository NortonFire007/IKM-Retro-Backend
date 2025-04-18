using System.ComponentModel.DataAnnotations;

namespace IKM_Retro.DTOs.User
{
    public class PatchUserProfileBody
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "User name must be between 3 and 50 characters")]
        public string ?UserName { get; set; }
        [EmailAddress]
        public string ?Email { get; set; }
    }
}
