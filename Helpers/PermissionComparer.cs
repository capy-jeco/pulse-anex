using portal_agile.Security;

namespace portal_agile.Helpers
{
    public class PermissionComparer : IEqualityComparer<Permission>
    {
        #pragma warning disable CS8767
        public bool Equals(Permission x, Permission y)
        #pragma warning restore CS8767 
        {
            return x.PermissionId == y.PermissionId;
        }

        public int GetHashCode(Permission obj)
        {
            return obj.PermissionId.GetHashCode();
        }
    }
}
