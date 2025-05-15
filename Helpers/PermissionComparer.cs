using portal_agile.Security;

namespace portal_agile.Helpers
{
    public class PermissionComparer : IEqualityComparer<Permission>
    {
        public bool Equals(Permission x, Permission y)
        {
            return x.PermissionId == y.PermissionId;
        }

        public int GetHashCode(Permission obj)
        {
            return obj.PermissionId.GetHashCode();
        }
    }
}
