using IKM_Retro.Data;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Repositories;

public class InviteRepository(RetroDbContext ctx) : BaseRepository(ctx)
 {
    private readonly RetroDbContext _ctx = ctx;
    

    public async Task<Retrospective?> GetRetrospectiveWithInviteAsync(Guid retrospectiveId,
        CancellationToken cancellationToken)
    {
        return await _ctx.Retrospectives
            .Include(r => r.InviteLink)
            .FirstOrDefaultAsync(r => r.Id == retrospectiveId, cancellationToken);
    }

    public async Task<RetrospectiveInvite?> GetActiveInviteAsync(string code, CancellationToken cancellationToken)
    {
        return await _ctx.RetrospectiveInvites
            .Include(i => i.Retrospective)
            .FirstOrDefaultAsync(
                i => i.Code == code && i.IsActive && (i.ExpiresAt == null || i.ExpiresAt > DateTime.UtcNow),
                cancellationToken);
    }

    public async Task<List<RetrospectiveInvite>> GetExpiredInvitesAsync(CancellationToken cancellationToken)
    {
        return await _ctx.RetrospectiveInvites
            .Where(i => i.IsActive && i.ExpiresAt != null && i.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public void RemoveExpiredInvites(List<RetrospectiveInvite> invites)
    {
        _ctx.RetrospectiveInvites.RemoveRange(invites);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _ctx.SaveChangesAsync(cancellationToken);
    }

    public void UpdateRetrospective(Retrospective retrospective)
    {
        _ctx.Retrospectives.Update(retrospective);
    }
    
    public async Task<RetrospectiveInvite?> GetActiveInviteByCode(string code)
    {
        return await _ctx.RetrospectiveInvites
            .Include(r => r.Retrospective)
            .FirstOrDefaultAsync(i => i.Code == code && i.IsActive && (i.ExpiresAt == null || i.ExpiresAt > DateTime.UtcNow));
    }
}