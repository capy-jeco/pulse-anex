using System.Security.Claims;
using portal_agile.Security;

namespace portal_agile.Contracts.Repositories
{
    public interface IPermissionRepository
    {
        /// <summary>
        /// Get all permissions
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Permission>> GetAllPermissions();

        /// <summary>
        /// Get permissions by permission id
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        Task<Permission> GetPermissionById(int permissionId);

        /// <summary>
        /// Get permissions by role id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<IEnumerable<Permission>> GetPermissionsByRoleId(string roleId);

        /// <summary>
        /// Get permissions by module
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, List<Permission>>> GetPermissionsByModule();

        /// <summary>
        /// Get permissions of role by role id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<List<Permission>> GetRolePermissions(string roleId);

        /// <summary>
        /// Get direct permissions of user by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<Permission>> GetUserDirectPermissions(string userId);

        /// <summary>
        /// Get all permissions of user by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<Permission>> GetAllUserPermissions(string userId);

        /// <summary>
        /// Create a new permission
        /// </summary>
        /// <returns></returns>
        Task<Permission> Store(Permission permission);

        /// <summary>
        /// Assign permissions to user
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionIds"></param>
        /// <param name="modifiedBy"></param>
        /// <returns></returns>
        Task<bool> AssignPermissionsToRole(string roleId, IEnumerable<int> permissionIds, string modifiedBy);

        /// <summary>
        /// Assign direct permissions to user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissionIds"></param>
        /// <param name="modifiedBy"></param>
        /// <returns></returns>
        Task<bool> AssignDirectPermissionsToUser(string userId, IEnumerable<int> permissionIds, string modifiedBy);

        /// <summary>
        /// Check if user has the specific permission
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissionCode"></param>
        /// <returns></returns>
        Task<bool> HasPermission(string userId, string permissionCode);

        /// <summary>
        /// Get user permission claims
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IList<Claim>> GetUserPermissionClaims(string userId);

        /// <summary>
        /// Seed default permissions
        /// </summary>
        /// <returns></returns>
        Task SeedDefaultPermissions();
    }
}
