using IKM_Retro.Data;

namespace IKM_Retro.Repositories.Base
{
    public class BaseRepository(RetroDbContext ctx)
    {
        private readonly RetroDbContext _ctx = ctx;
        public async Task SaveChangesAsync() => await _ctx.SaveChangesAsync();
    }
}
