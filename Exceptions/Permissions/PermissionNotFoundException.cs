namespace portal_agile.Exceptions.Permissions
{
    public class PermissionNotFoundException : Exception
    {
        public PermissionNotFoundException() : base("Permission not found.")
        {
        }

        public PermissionNotFoundException(string message) : base(message)
        {
        }

        public PermissionNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public int PermissionId { get; set; }

        public PermissionNotFoundException(int permissionId) : base($"Permission with ID {permissionId} not found.")
        {
            PermissionId = permissionId;
        }
    }
}
