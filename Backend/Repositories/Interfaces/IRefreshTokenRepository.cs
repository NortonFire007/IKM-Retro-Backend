using IKM_Retro.DTOs.Auth;

namespace IKM_Retro.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        public Task<JwtToken> GenerateTokensAsync(string userName);
    }
}
