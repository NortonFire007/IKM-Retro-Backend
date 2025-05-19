using Microsoft.AspNetCore.Authorization;
using IKM_Retro.Services;

namespace IKM_Retro.Authorization;

public class RetrospectiveRoleHandler(RoleService roleService, IHttpContextAccessor httpContextAccessor)
    : AuthorizationHandler<RetrospectiveRoleRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        RetrospectiveRoleRequirement requirement)
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        var userId = httpContext.User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userId)) return;

        var routeValue = httpContext.GetRouteValue("retrospectiveId")?.ToString();
        if (!Guid.TryParse(routeValue, out Guid retrospectiveId)) return;

        var role = await roleService.GetUserRoleAsync(userId, retrospectiveId);
        if (role is not null && requirement.AllowedRoles.Contains(role.Value))
        {
            context.Succeed(requirement);
        }
    }

}