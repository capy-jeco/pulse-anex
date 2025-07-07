using portal_agile.Dtos.Modules;
using portal_agile.Security;
using System.Security.Claims;

namespace portal_agile.Contracts.Repositories
{
    public interface IPermissionRepository : IBaseRepository<Permission, int>
    {
        /// <summary>
        /// Get permissions by role id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<IEnumerable<Permission>> GetPermissionsByRoleId(string roleId);

        /// <summary>
        /// Get permissions by roles
        /// </summary>
        /// <param name="roles"></param>
        /// <returns>List of Permission</returns>
        Task<List<Permission>> GetPermissionsByRolesAsync(IList<string> roles);

        /// <summary>
        /// Assign permissions to user
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionIds"></param>
        /// <param name="modifiedBy"></param>
        /// <returns></returns>
        Task<bool> AssignPermissionsToRole(string roleId, IEnumerable<int> permissionIds, string modifiedBy);

        /// <summary>
        /// Check if user has the specific permission
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissionCode"></param>
        /// <returns></returns>
        Task<bool> HasPermission(string userId, string permissionCode);
    }
}
