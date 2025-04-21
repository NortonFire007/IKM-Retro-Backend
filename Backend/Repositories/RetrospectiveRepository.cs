using IKM_Retro.Data;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Repositories
{
    public class RetrospectiveRepository(RetroDbContext ctx) : BaseRepository(ctx)
    {
        private readonly RetroDbContext _ctx = ctx;

        public async Task<Retrospective?> GetById(Guid id)
        {
            return await _ctx.Retrospectives.FindAsync(id);
        }

        public async Task<List<Retrospective>> GetByUserId(string userId)
        {
            return await _ctx.RetrospectiveToUser
                    .Where(rtu => rtu.UserId == userId)
                    .Select(rtu => rtu.Retrospective)
                    .ToListAsync();
        }
        
        public async Task<int> CountUserRetrospectives(string userId)
        {
            return await _ctx.RetrospectiveToUser.Where(rtu => rtu.UserId == userId).CountAsync();
        }

        public async Task Add(Retrospective retrospective)
        {
            await _ctx.Retrospectives.AddAsync(retrospective);
        }
    }
}
