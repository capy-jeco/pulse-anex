using Microsoft.AspNetCore.Authorization;
using portal_agile.Authorization.Requirements;
using portal_agile.Contracts.Services;

namespace portal_agile.Authorization.Handlers
{
    public class PermissionAuthorizationHandler(IPermissionService permissionService) : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService = permissionService;

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if ((bool)!context.User.Identity?.IsAuthenticated!)
            {
                return;
            }

            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // Check if user has the required permission
            if (await _permissionService.HasPermissionAsync(userId, requirement.PermissionCode))
            {
                context.Succeed(requirement);
            }
        }
    }
}
