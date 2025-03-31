using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IKM_Retro.DTOs.Auth
{
    public class JwtToken(string accessToken, string refreshToken)
    {
        public string AccessToken { get; set; } = accessToken;
        public string RefreshToken { get; set; } = refreshToken;
    }
}
