using IKM_Retro.Data;
using IKM_Retro.Enums;
using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Services;

public class RoleService
{
    private readonly RetroDbContext _context;

    public RoleService(RetroDbContext context)
    {
        _context = context;
    }

    public async Task<RoleTypeEnum?> GetUserRoleAsync(string userId, Guid retrospectiveId)
    {
        var roleEntry = await _context.RetrospectiveToUser
            .FirstOrDefaultAsync(r => r.UserId == userId && r.RetrospectiveId == retrospectiveId);
        return roleEntry?.Role;
    }
}
