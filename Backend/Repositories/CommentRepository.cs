using IKM_Retro.Data;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Repositories;

public class CommentRepository(RetroDbContext ctx) : BaseRepository(ctx)
{
    private readonly RetroDbContext _ctx = ctx;

    public void Create(Comment comment)
    {
        _ctx.Comments.Add(comment);
    }

    public void Update(Comment comment)
    {
        _ctx.Comments.Update(comment);
    }

    public void Delete(Comment comment)
    {
        _ctx.Comments.Remove(comment);
    }

    public async Task<Comment?> GetByIdAsync(int commentId)
    {
        return await _ctx.Comments
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == commentId);
    }

    public async Task<List<Comment>> GetByGroupItemIdAsync(int groupItemId)
    {
        return await _ctx.Comments
            .Where(x => x.GroupItemId == groupItemId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }
}