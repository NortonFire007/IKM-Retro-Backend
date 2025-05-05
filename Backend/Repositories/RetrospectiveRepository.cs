using IKM_Retro.Data;
using IKM_Retro.DTOs.Retrospective;
using IKM_Retro.Exceptions.Base;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Repositories;

public class RetrospectiveRepository(RetroDbContext ctx) : BaseRepository(ctx)
{
    private readonly RetroDbContext _ctx = ctx;
    
    public async Task<Retrospective?> GetById(Guid id)
    {
        return await _ctx.Retrospectives.FindAsync(id);
    }
    public async Task<Retrospective> GetByIdOr404Async(int retrospectiveId)
    {
        Retrospective? retrospective = await _ctx.Retrospectives.FindAsync(retrospectiveId);

        if (retrospective == null)
            throw new NotFoundException($"Retrospective with ID {retrospectiveId} not found");

        return retrospective;
    }

    public async Task<List<RetrospectiveToUserDto>> GetByUserId(string userId)
    {
        return await _ctx.RetrospectiveToUser
            .Where(rtu => rtu.UserId == userId)
            .ProjectToType<RetrospectiveToUserDto>()
            .ToListAsync();
        // return await _ctx.RetrospectiveToUser
        //         .Where(rtu => rtu.UserId == userId)
        //         .Select(rtu => rtu.Retrospective)
        //         .ToListAsync();
    }
        
    public async Task<int> CountUserRetrospectives(string userId)
    {
        return await _ctx.RetrospectiveToUser.Where(rtu => rtu.UserId == userId).CountAsync();
    }

    public async Task Add(Retrospective retrospective)
    {
        await _ctx.Retrospectives.AddAsync(retrospective);
    }

    public async Task AddRelation(RetrospectiveToUser retrospectiveToUser)
    {
        await _ctx.RetrospectiveToUser.AddAsync(retrospectiveToUser);
    }
        
    public async Task<bool> IsUserInRetrospective(Guid retrospectiveId, string userId)
    {
        return await _ctx.RetrospectiveToUser.AnyAsync(rtu => rtu.RetrospectiveId == retrospectiveId && rtu.UserId == userId);
    }
        
    public void Delete(Retrospective retrospective)
    {
        _ctx.Retrospectives.Remove(retrospective);
    }
    
    public async Task<Retrospective?> GetRetrospectiveWithInviteAsync(Guid retrospectiveId,
        CancellationToken cancellationToken)
    {
        return await _ctx.Retrospectives
            .Include(r => r.InviteLink)
            .FirstOrDefaultAsync(r => r.Id == retrospectiveId, cancellationToken);
    }
    
    public IQueryable<RetrospectiveToUser> GetRetrospectiveUsersQuery()
    {
        return _ctx.RetrospectiveToUser.AsQueryable();
    }
}