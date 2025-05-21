namespace portal_agile.Dtos.Roles
{
    public class RoleDto
    {
        public required string Id { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public required bool IsSystemRole { get; set; } = false;
    }
}
