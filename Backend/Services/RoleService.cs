using IKM_Retro.Data;
using IKM_Retro.Enums;
using IKM_Retro.Models.Retro;
using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Services;

public class RoleService(RetroDbContext context)
{
    public async Task<RoleTypeEnum?> GetUserRoleAsync(string userId, Guid retrospectiveId)
    {

        return (await context.RetrospectiveToUser
            .FirstOrDefaultAsync(r => r.UserId == userId && r.RetrospectiveId == retrospectiveId))?.Role;
    }
}
