using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Services;
using portal_agile.Data;
using portal_agile.Models;

namespace portal_agile.Services
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;
        private int _currentTenantId;

        public TenantService(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }

        public int GetCurrentTenantId()
        {
            return _currentTenantId;
        }

        public async Task<Tenant?> GetCurrentTenantAsync()
        {
            if (_currentTenantId == 0) return null;

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            return await context.Tenants
                .Where(t => t.Id == _currentTenantId && t.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<Tenant?> GetTenantBySubdomainAsync(string subdomain)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            return await context.Tenants
                .Where(t => t.Subdomain == subdomain && t.IsActive)
                .FirstOrDefaultAsync();
        }

        public void SetCurrentTenant(int tenantId)
        {
            _currentTenantId = tenantId;
        }
    }
}
