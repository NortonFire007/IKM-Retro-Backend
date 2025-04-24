using IKM_Retro.Data;

namespace IKM_Retro.Repositories.Base;

public class BaseRepository(RetroDbContext ctx)
{
    public async Task SaveChangesAsync() => await ctx.SaveChangesAsync();
}