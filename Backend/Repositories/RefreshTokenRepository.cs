﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IKM_Retro.Data;
using IKM_Retro.DTOs.Auth;
using IKM_Retro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IKM_Retro.Repositories
{
    public class RefreshTokenRepository(IOptions<JwtOptions> jwtOptions, RetroDbContext ctx)
    {
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;
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
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([new Claim("userId", userId)]),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessToken.ExpirationTimeMinutes),
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            var existingRefreshTokens = _ctx.RefreshTokens.Where(t => t.UserId == userId);
            _ctx.RefreshTokens.RemoveRange(existingRefreshTokens);

            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var refreshTokenEntry = new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtOptions.RefreshToken.ExpirationTimeDays)
            };

            _ctx.RefreshTokens.Add(refreshTokenEntry);

            await _ctx.SaveChangesAsync();

            return new JwtToken(accessToken, refreshToken);

        }

    }
}
