using portal_agile.Dtos.Modules;

namespace portal_agile.Dtos.Users.Response
{
    public class UserAuthResponseDto
    {
        public required string Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        public IEnumerable<ModulePermissionDto> Modules { get; set; } = new List<ModulePermissionDto>();
    }
}
