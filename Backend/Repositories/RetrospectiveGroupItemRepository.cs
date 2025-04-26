using IKM_Retro.Data;
using IKM_Retro.DTOs.Retrospective.Group.Items;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Repositories
{
    public class RetrospectiveGroupItemRepository(RetroDbContext ctx) : BaseRepository(ctx)
    {
        private readonly RetroDbContext _ctx = ctx;

        public async Task<GroupItem?> GetById(int id)
        {
            return await _ctx.GroupItems.FindAsync(id);
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


        public async Task Add(GroupItem groupItem)
        {
            await _ctx.GroupItems.AddAsync(groupItem);
        }
    }
}
