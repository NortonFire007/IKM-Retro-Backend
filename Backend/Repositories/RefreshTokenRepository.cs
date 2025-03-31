using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IKM_Retro.Data;
using IKM_Retro.DTOs.Auth;
using IKM_Retro.Models;
using IKM_Retro.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace IKM_Retro.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IConfiguration _configuration;
        private readonly RetroDbContext _ctx;

        public RefreshTokenRepository(IConfiguration configuration, RetroDbContext ctx)
        {
            _configuration = configuration;
            _ctx = ctx;
        }

        public async Task<JwtToken> GenerateTokensAsync(string userName)
        {
            var secret = _configuration["Jwt:Secret"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (secret is null || issuer is null || audience is null)
            {
                throw new ApplicationException("JWT settings are missing in the configuration");
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshTokenEntry = new RefreshToken
            {
                Token = refreshToken,
                UserName = userName,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            _ctx.RefreshTokens.Add(refreshTokenEntry);

            await _ctx.SaveChangesAsync();

            return new JwtToken(accessToken, refreshToken);

        }
    }
}
