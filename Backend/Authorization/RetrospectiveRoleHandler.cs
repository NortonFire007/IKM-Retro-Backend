using Microsoft.AspNetCore.Authorization;
using IKM_Retro.Services;

namespace IKM_Retro.Authorization;

public class RetrospectiveRoleHandler : AuthorizationHandler<RetrospectiveRoleRequirement>
{
    private readonly RoleService _roleService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RetrospectiveRoleHandler(RoleService roleService, IHttpContextAccessor httpContextAccessor)
    {
        _roleService = roleService;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        RetrospectiveRoleRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return;

        var userId = httpContext.User.FindFirst("userId")?.Value;

        var retrospectiveIdObj = httpContext.GetRouteValue("id");
        if (retrospectiveIdObj == null) return;

        if (!Guid.TryParse(retrospectiveIdObj.ToString(), out var retrospectiveId))
            return;

        var role = await _roleService.GetUserRoleAsync(userId, retrospectiveId);

        if (role != null && requirement.AllowedRoles.Contains(role.Value))
        {
            context.Succeed(requirement);
        }
    }
}