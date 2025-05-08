using IKM_Retro.Enums;
using Microsoft.AspNetCore.Authorization;

namespace IKM_Retro.Authorization;

public class RetrospectiveRoleRequirement : IAuthorizationRequirement
{
    public RoleTypeEnum[] AllowedRoles { get; }

    public RetrospectiveRoleRequirement(params RoleTypeEnum[] allowedRoles)
    {
        AllowedRoles = allowedRoles;
    }
}