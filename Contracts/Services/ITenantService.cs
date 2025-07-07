namespace portal_agile.Contracts.Services
{
    public interface ITenantService
    {
        int GetCurrentTenantId();
        Task<Models.Tenant?> GetCurrentTenantAsync();
        Task<Models.Tenant?> GetTenantBySubdomainAsync(string subdomain);
        void SetCurrentTenant(int tenantId);
    }
}
