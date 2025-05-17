namespace portal_agile.Dtos.Permissions
{
    public class PermissionDto
    {
        public required int PermissionId { get; set; }

        public required string Name { get; set; }

        public required string Code { get; set; }

        public required string Module { get; set; }

        public required string Description { get; set; }
    }
}
