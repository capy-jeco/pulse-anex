using portal_agile.Dtos.Permissions;
using portal_agile.Security;
using System.Security.Claims;

namespace portal_agile.Contracts.Services
{
    public interface IPermissionService
    {
        /// <summary>
        /// Get all permissions
        /// </summary>
        /// <returns> List of all permissions </returns>
        Task<List<PermissionDto>> GetAllPermissionsAsync();

        /// <summary>
        /// Get permissions by permission id
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        Task<PermissionDto> GetPermissionByIdAsync(int permissionId);

        /// <summary>
        /// Get permissions of a role by role id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<List<PermissionDto>> GetRolePermissionsAsync(string roleId);

        /// <summary>
        /// Create a new permission
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<PermissionDto> CreateAsync(PermissionCreateDto permission);

        /// <summary>
        /// Update a permission
        /// </summary>
        /// <param name="permissionUpdate"></param>
        /// <returns></returns>
        Task<PermissionDto> UpdatePermissionAsync(Permission permissionUpdate);

        /// <summary>
        /// Assign permissions to role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionIds"></param>
        /// <param name="modifiedBy"></param>
        /// <returns></returns>
        Task<bool> AssignPermissionsToRoleAsync(string roleId, IEnumerable<int> permissionIds, string modifiedBy);

        /// <summary>
        /// Check if user has specific permission
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissionCode"></param>
        /// <returns></returns>
        Task<bool> HasPermissionAsync(string userId, string permissionCode);
    }
}
