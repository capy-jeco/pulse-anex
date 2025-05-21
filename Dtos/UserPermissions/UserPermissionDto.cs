using portal_agile.Models;
using portal_agile.Security;

namespace portal_agile.Dtos.UserPermissions
{
    public class UserPermissionDto
    {
        public required int UserPermissionId { get; set; }
        public required string UserId { get; set; }
        public int PermissionId { get; set; }
        public virtual User? User { get; set; }
        public virtual Permission? Permission { get; set; }
    }
}
