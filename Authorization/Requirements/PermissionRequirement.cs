using Microsoft.AspNetCore.Authorization;

namespace portal_agile.Authorization.Requirements
{
    public class PermissionRequirement(string permissionCode) : IAuthorizationRequirement
    {
        public string PermissionCode { get; } = permissionCode;
    }
}
