using portal_agile.Dtos.MenuItem;
using portal_agile.Dtos.Modules;

namespace portal_agile.Dtos.Users.Response
{
    public class UserMenuResponse
    {
        public List<ModulePermissionDto> Modules { get; set; } = new List<ModulePermissionDto>();
        public List<MenuItemDto> Menu { get; set; } = new List<MenuItemDto>();
        public List<string> UserPermissions { get; set; } = new List<string>();
    }
}
