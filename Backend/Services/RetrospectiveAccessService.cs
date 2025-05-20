using IKM_Retro.Data;
using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Services;

public class RetrospectiveAccessService(RetroDbContext ctx)
{
    public async Task<string?> GetOwnerIdAsync(Guid retrospectiveId)
    {
        return await ctx.Retrospectives
            .Where(r => r.Id == retrospectiveId)
            .Select(r => (string?)r.CreatorUserId)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsOwnerAsync(string userId, Guid retrospectiveId)
    {
        var ownerId = await GetOwnerIdAsync(retrospectiveId);
        return ownerId == userId;
    }

    public async Task<bool> IsParticipantAsync(string userId, Guid retrospectiveId)
    {
        return await ctx.RetrospectiveToUser
            .AnyAsync(ru => ru.RetrospectiveId == retrospectiveId && ru.UserId == userId);
    }

    public async Task<bool> HasAccessAsync(string userId, Guid retrospectiveId)
    {
        return await IsOwnerAsync(userId, retrospectiveId) || await IsParticipantAsync(userId, retrospectiveId);
    }
}