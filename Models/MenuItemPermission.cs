using portal_agile.Security;
using System.ComponentModel.DataAnnotations;

namespace portal_agile.Models
{
    public class MenuItemPermission
    {
        public int MenuItemId { get; set; }
        public int PermissionId { get; set; }

        // Navigation Properties
        public MenuItem MenuItem { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }
}
