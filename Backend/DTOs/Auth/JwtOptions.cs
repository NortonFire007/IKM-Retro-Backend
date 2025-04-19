namespace IKM_Retro.DTOs.Auth
{

    public class AccessTokenOptions
    {
        public int ExpirationTimeMinutes { get; set; }
    }
    public class RefreshTokenOptions
    {
        public int ExpirationTimeDays { get; set; }
    }

    public class JwtClaims
    {
        public string Sub { get; set; } = string.Empty;
    }
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string CookieName { get; set; } = string.Empty;
        public string Sub { get; set; } = string.Empty;
        public AccessTokenOptions AccessToken { get; set; }
        public RefreshTokenOptions RefreshToken { get; set; }
    }
}
