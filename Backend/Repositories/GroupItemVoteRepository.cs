using IKM_Retro.Data;
using IKM_Retro.DTOs.Retrospective.Group.Items;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Repositories;

public class GroupItemVoteRepository(RetroDbContext ctx) : BaseRepository(ctx)
{
    private readonly RetroDbContext _ctx = ctx;


    public async Task<GroupItemVote?> GetByUserIdAndGroupItemId(string userId, int groupItemId)
    {
        return await _ctx.GroupItemVotes
            .Where(giv => giv.GroupItemId == groupItemId && giv.UserId == userId)
            .FirstOrDefaultAsync();

    }
    
    public async Task<int> GetTotalVotesByUserAndRetrospective(string userId, Guid retrospectiveId)
    {
        return await _ctx.GroupItemVotes
            .Where(giv => giv.UserId == userId &&
                          _ctx.Groups.Any(g => g.Id == giv.GroupItem.GroupId && g.RetrospectiveId == retrospectiveId))
            .SumAsync(giv => giv.Count);
    }



    public async Task Add(GroupItemVote groupItemVote)
    {
        await _ctx.AddAsync(groupItemVote);
    }

    public void Remove(GroupItemVote groupItemVote)
    {
        _ctx.Remove(groupItemVote);
    }

    public IQueryable<GroupItemVote> GetAllQuery()
    {
        return _ctx.GroupItemVotes.AsQueryable();
    }
}