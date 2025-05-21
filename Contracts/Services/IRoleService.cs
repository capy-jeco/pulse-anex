using portal_agile.Dtos.Permissions;
using portal_agile.Dtos.Roles;
using portal_agile.Security;

namespace portal_agile.Contracts.Services
{
    public interface IRoleService
    {
        /// <summary>
        /// Gets all roles
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();

        /// <summary>
        /// Gets a role by its ID
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<RoleDto> GetRoleByIdAsync(string roleId);

        /// <summary>
        /// Get permissions of role by role id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<List<PermissionDto>?> GetRolePermissionsAsync(string roleId);

        /// <summary>
        /// Creates a new role
        /// </summary>
        /// <param name="roleCreateDto"></param>
        /// <returns></returns>
        Task<RoleDto> CreateRoleAsync(RoleCreateDto roleCreateDto);

        /// <summary>
        /// Updates a role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roleUpdate"></param>
        /// <returns></returns>
        Task<RoleDto> UpdateRoleAsync(string roleId, RoleDto roleUpdate);

        /// <summary>
        /// Soft deletes a role
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<bool> SoftDeleteRoleAsync(string roleId);

        /// <summary>
        /// Checks if a role exists by its ID
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<bool> RoleExistsAsync(string roleId);
    }
}
