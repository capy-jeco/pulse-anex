using portal_agile.Dtos.Permissions;

namespace portal_agile.Dtos.Users
{
    public class UserWithPermissionsDto : UserDto
    {
        public string? UserId { get; set; }

        public IEnumerable<PermissionDto>? UserPermission { get; set; }

    }
}
