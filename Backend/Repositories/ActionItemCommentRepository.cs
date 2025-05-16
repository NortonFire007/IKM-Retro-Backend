using IKM_Retro.Data;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Repositories;

public class ActionItemCommentRepository(RetroDbContext ctx) : BaseRepository(ctx)
{
    private readonly RetroDbContext _ctx = ctx;
    
    public async Task<ActionItemComment?> GetByIdAsync(int commentId)
    {
        return await _ctx.ActionItemComments
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == commentId);
    }

    public async Task<List<ActionItemComment>> GetByActionItemIdAsync(int actionItemId)
    {
        return await _ctx.ActionItemComments
            .Where(x => x.ActionItemId == actionItemId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public void Create(ActionItemComment actionItemComment)
    {
        _ctx.ActionItemComments.Add(actionItemComment);
    }

    public void Update(ActionItemComment actionItemComment)
    {
        _ctx.ActionItemComments.Update(actionItemComment);
    }

    public void Delete(ActionItemComment actionItemComment)
    {
        _ctx.ActionItemComments.Remove(actionItemComment);
    }
}
