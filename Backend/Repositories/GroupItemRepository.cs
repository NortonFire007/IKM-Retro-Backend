using IKM_Retro.Data;
using IKM_Retro.DTOs.Retrospective.Group.Items;
using IKM_Retro.Exceptions.Base;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories.Base;
using Mapster;
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

    public void DeleteAsync(GroupItem item)
    {
        _ctx.GroupItems.Remove(item);
    }

    public async Task<int> CountByGroupId(int groupId)
    {
        return await _ctx.GroupItems.Where(gi => gi.GroupId == groupId).CountAsync();
    }
    
    public async Task<GroupItem> GetByIdOr404Async(int groupItemId)
    {
        GroupItem? groupItem = await _ctx.GroupItems.FindAsync(groupItemId);

        if (groupItem == null)
            throw new NotFoundException($"Group item with ID {groupItemId} not found");

        return groupItem;
    }

    public async Task<List<BaseGroupItemDTO>> GetByRetrospectiveId(Guid retrospectiveId)
    {
        return await _ctx.GroupItems
            .Where(gi => gi.Group.RetrospectiveId == retrospectiveId)
            .ProjectToType<BaseGroupItemDTO>()
            .ToListAsync();
    }

    public async Task<List<BaseGroupItemDTO>> GetByGroupId(int groupId)
    {
        return await _ctx.GroupItems
            .Where(gi => gi.GroupId == groupId)
            .ProjectToType<BaseGroupItemDTO>()
            .ToListAsync();
    }

    public async Task<GroupItem?> GetByIdAsync(int groupItemId)
    {
        return await _ctx.GroupItems
            .FirstOrDefaultAsync(x => x.Id == groupItemId);
    }
    public async Task ShiftOrderPositionsForInsert(int groupId, int fromPosition)
    {
        await _ctx.GroupItems
            .Where(x => x.GroupId == groupId && x.OrderPosition >= fromPosition)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.OrderPosition, x => x.OrderPosition + 1));
    }

    public async Task ShiftOrderPositionsForRemove(int groupId, int fromPosition)
    {
        await _ctx.GroupItems
            .Where(x => x.GroupId == groupId && x.OrderPosition > fromPosition)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.OrderPosition, x => x.OrderPosition - 1));
    }

    public async Task ShiftOrderPositionsWithinGroup(int groupId, int oldPosition, int newPosition)
    {
        if (newPosition < oldPosition)
        {
            await _ctx.GroupItems
                .Where(x => x.GroupId == groupId && x.OrderPosition >= newPosition && x.OrderPosition < oldPosition)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.OrderPosition, x => x.OrderPosition + 1));
        }
        else if (newPosition > oldPosition)
        {
            await _ctx.GroupItems
                .Where(x => x.GroupId == groupId && x.OrderPosition <= newPosition && x.OrderPosition > oldPosition)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.OrderPosition, x => x.OrderPosition - 1));
        }
    }

    public IQueryable<GroupItem> GetAllQuery()
    {
        return _ctx.GroupItems.AsQueryable();
    }
}
