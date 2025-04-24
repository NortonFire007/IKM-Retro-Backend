using IKM_Retro.Data;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Repositories;

public class GroupItemRepository(RetroDbContext ctx) : BaseRepository(ctx)
{
    private readonly RetroDbContext _ctx = ctx;

    public async Task CreateAsync(GroupItem groupItem)
    {
        await _ctx.GroupItems.AddAsync(groupItem);
    }

    public void Update(GroupItem groupItem)
    {
        _ctx.GroupItems.Update(groupItem);
    }

    public async Task DeleteAsync(int groupItemId)
    {
        var item = await _ctx.GroupItems.FirstOrDefaultAsync(x => x.Id == groupItemId);
        if (item != null)
        {
            _ctx.GroupItems.Remove(item);
        }
    }

    public async Task<List<GroupItem>> GetByRetrospectiveIdAsync(Guid retrospectiveId)
    {
        return await _ctx.GroupItems
            .Include(x => x.Group)
            .Where(x => x.Group.RetrospectiveId == retrospectiveId)
            .OrderBy(x => x.Group.OrderPosition)
            .ThenBy(x => x.OrderPosition)
            .ToListAsync();
    }

    public async Task<GroupItem?> GetByIdAsync(int groupItemId)
    {
        return await _ctx.GroupItems
            .Include(x => x.Group)
            .FirstOrDefaultAsync(x => x.Id == groupItemId);
    }
}
