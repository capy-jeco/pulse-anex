using portal_agile.Contracts.Services;
using portal_agile.Models;

namespace portal_agile.Middlewares
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
        {
            var host = context.Request.Host.Host;

            // Extract subdomain (assuming format: agile.pulse.anex.com)
            var parts = host.Split('.');
            if (parts.Length >= 3)
            {
                var subdomain = parts[0];
                var tenant = await tenantService.GetTenantBySubdomainAsync(subdomain);

                if (tenant != null)
                {
                    tenantService.SetCurrentTenant(tenant.Id);
                    context.Items["Tenant"] = tenant;
                }
            }

            await _next(context);
        }
    }
}
