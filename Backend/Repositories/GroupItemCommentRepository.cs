using IKM_Retro.Data;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Repositories;

public class GroupItemCommentRepository(RetroDbContext ctx) : BaseRepository(ctx)
{
    private readonly RetroDbContext _ctx = ctx;
    
    public async Task<GroupItemComment?> GetByIdAsync(int commentId)
    {
        return await _ctx.GroupItemComments
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == commentId);
    }

    public async Task<List<GroupItemComment>> GetByGroupItemIdAsync(int groupItemId)
    {
        return await _ctx.GroupItemComments
            .Where(x => x.GroupItemId == groupItemId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public void Create(GroupItemComment groupItemComment)
    {
        _ctx.GroupItemComments.Add(groupItemComment);
    }

    public void Update(GroupItemComment groupItemComment)
    {
        _ctx.GroupItemComments.Update(groupItemComment);
    }

    public void Delete(GroupItemComment groupItemComment)
    {
        _ctx.GroupItemComments.Remove(groupItemComment);
    }
}