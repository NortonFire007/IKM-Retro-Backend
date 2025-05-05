using IKM_Retro.Data;
using IKM_Retro.Exceptions.Base;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Repositories;

public class RetrospectiveGroupRepository(RetroDbContext ctx) : BaseRepository(ctx)
{
    private readonly RetroDbContext _ctx = ctx;

    public async Task<Group?> GetById(int id)
    {
        return await _ctx.Groups.FindAsync(id);
    }
    public async Task<Group> GetByIdOr404Async(int groupId)
    {
        Group? group = await _ctx.Groups.FindAsync(groupId);

        if (group == null)
            throw new NotFoundException($"Group with ID {groupId} not found");

        return group;
    }
    public async Task<List<BaseGroupDTO>> GetByRetrospectiveId(Guid retrospectiveId)
    {
        return await _ctx.Groups
            .Where(g => g.RetrospectiveId == retrospectiveId)
            .ProjectToType<BaseGroupDTO>()
            .ToListAsync();
    }

    public async Task Add(Group group)
    {
        await _ctx.Groups.AddAsync(group);
    }

    public IQueryable<Group> GetAllQuery()
    {
        return _ctx.Groups.AsQueryable();
    }
}