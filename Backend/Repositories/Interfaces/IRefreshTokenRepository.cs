using IKM_Retro.DTOs.Auth;
using IKM_Retro.Models;

namespace IKM_Retro.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        public Task<RefreshToken?> GetAsync(string refreshToken);
        public void Remove(RefreshToken refreshToken);
        public Task<JwtToken> GenerateTokensAsync(string userId);
    }
}
