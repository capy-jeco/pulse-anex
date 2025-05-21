using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using portal_agile.Authorization.Requirements;

namespace portal_agile.Policy.Provider
{
    public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; } = new DefaultAuthorizationPolicyProvider(options);

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return FallbackPolicyProvider.GetFallbackPolicyAsync()!;
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // If the policy name doesn't start with "Permission:", use the fallback provider
            if (!policyName.StartsWith("Permission:"))
            {
                var policyTask = FallbackPolicyProvider.GetPolicyAsync(policyName);
                
                _ = policyTask!.ContinueWith(task =>
                {
                    if (task.Result == null)
                    {
                        throw new InvalidOperationException($"Policy '{policyName}' cannot be null.");
                    }
                    return task.Result;
                });
            }

            // Extract the permission code from the policy name
            var permissionCode = policyName.Substring("Permission:".Length);

            // Create a policy requiring the permission
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(permissionCode))
                .Build();

            return Task.FromResult(policy)!;
        }
    }
}
