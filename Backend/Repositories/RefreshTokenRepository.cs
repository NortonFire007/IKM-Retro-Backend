using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IKM_Retro.Data;
using IKM_Retro.DTOs.Auth;
using IKM_Retro.Models;
using IKM_Retro.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace IKM_Retro.Repositories
{
    public class RefreshTokenRepository(IConfiguration configuration, RetroDbContext ctx) : IRefreshTokenRepository
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly RetroDbContext _ctx = ctx;

        public async Task<RefreshToken?> GetAsync(string refreshToken)
        {
            return await _ctx.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        }

        public void Remove(RefreshToken refreshToken)
        {
            _ctx.RefreshTokens.Remove(refreshToken);
        }

        public async Task<JwtToken> GenerateTokensAsync(string userId)
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

            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:AccessToken:ExpirationTimeMinutes"]));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId)]),
                Expires = accessTokenExpiry,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            var existingRefreshTokens = _ctx.RefreshTokens.Where(t => t.UserId == userId);
            _ctx.RefreshTokens.RemoveRange(existingRefreshTokens);

            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshTokenExpiry = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:RefreshToken:ExpirationTimeDays"]));

            var refreshTokenEntry = new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                ExpiryDate = refreshTokenExpiry
            };

            _ctx.RefreshTokens.Add(refreshTokenEntry);

            await _ctx.SaveChangesAsync();

            return new JwtToken(accessToken, refreshToken);

        }

    }
}
