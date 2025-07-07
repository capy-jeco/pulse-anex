using portal_agile.Dtos.Permissions;

namespace portal_agile.Dtos.Modules
{
    public class ModulePermissionDto
    {
        public required string ModuleName { get; set; }
        public required string DisplayName { get; set; }
        public required string Route { get; set; }
        public IEnumerable<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }
}
