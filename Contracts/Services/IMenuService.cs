using portal_agile.Dtos.MenuItem;
using portal_agile.Dtos.Modules;
using portal_agile.Dtos.Users.Response;

namespace portal_agile.Contracts.Services
{
    public interface IMenuService
    {
        Task<UserMenuResponse> GetUserMenuAsync(string userId);
    }
}
