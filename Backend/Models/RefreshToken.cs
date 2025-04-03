using System.ComponentModel.DataAnnotations.Schema;

namespace IKM_Retro.Models
{
    [Table("AspNetUserRefreshTokens")]
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; } = false;
    }

}
