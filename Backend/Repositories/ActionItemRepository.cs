using IKM_Retro.Data;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Repositories;

public class ActionItemRepository(RetroDbContext ctx) : BaseRepository(ctx)
{
    private readonly RetroDbContext _ctx = ctx;

    public async Task<List<ActionItem>> GetByRetrospectiveIdAsync(Guid retrospectiveId, CancellationToken cancellationToken)
    {
        return await _ctx.ActionItems
            .Where(a => a.RetrospectiveId == retrospectiveId)
            .Include(a => a.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<ActionItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _ctx.ActionItems
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.ActionId == id, cancellationToken);
    }

    public async Task AddAsync(ActionItem item, CancellationToken cancellationToken)
    {
        await _ctx.ActionItems.AddAsync(item, cancellationToken);
    }

    public void Update(ActionItem item)
    {
        _ctx.ActionItems.Update(item);
    }

    public void Remove(ActionItem item)
    {
        _ctx.ActionItems.Remove(item);
    }
}