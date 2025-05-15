using System.Security.Claims;
using portal_agile.Contracts.Repositories;
using portal_agile.Contracts.Services;
using portal_agile.Security;

namespace portal_agile.Services
{
    public class PermissionService(IPermissionRepository permissionRepository) : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository = permissionRepository;

        /// <inheritdoc/>
        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            return (List<Permission>)await _permissionRepository.GetAllPermissions();
        }

        public async Task<Permission> GetPermissionByIdAsync(int permissionId)
        {
            return await _permissionRepository.GetPermissionById(permissionId);
        }

        /// <inheritdoc/>
        public Task<List<Permission>> GetAllUserPermissionsAsync(string userId)
        {
            return _permissionRepository.GetAllUserPermissions(userId);
        }

        /// <inheritdoc/>
        public Task<Dictionary<string, List<Permission>>> GetPermissionsByModuleAsync()
        {
            return _permissionRepository.GetPermissionsByModule();
        }

        /// <inheritdoc/>
        public async Task<List<Permission>> GetRolePermissionsAsync(string roleId)
        {
            return await _permissionRepository.GetRolePermissions(roleId);
        }

        /// <inheritdoc/>
        public async Task<List<Permission>> GetUserDirectPermissionsAsync(string userId)
        {
            return await (_permissionRepository.GetUserDirectPermissions(userId));
        }

        /// <inheritdoc/>
        public async Task<IList<Claim>> GetUserPermissionClaimsAsync(string userId)
        {
            return await _permissionRepository.GetUserPermissionClaims(userId);
        }

        /// <inheritdoc/>
        public async Task<Permission> Store(Permission permission)
        {
            return await _permissionRepository.Store(permission);
        }

        /// <inheritdoc/>
        public async Task<bool> AssignDirectPermissionsToUserAsync(string userId, IEnumerable<int> permissionIds, string modifiedBy)
        {
            return await _permissionRepository.AssignDirectPermissionsToUser(userId, permissionIds, modifiedBy);
        }

        /// <inheritdoc/>
        public async Task<bool> AssignPermissionsToRoleAsync(string roleId, IEnumerable<int> permissionIds, string modifiedBy)
        {
            return await _permissionRepository.AssignPermissionsToRole(roleId, permissionIds, modifiedBy);
        }

        /// <inheritdoc/>
        public async Task<bool> HasPermissionAsync(string userId, string permissionCode)
        {
            return await _permissionRepository.HasPermission(userId, permissionCode);
        }

        /// <inheritdoc/>
        public async Task SeedDefaultPermissionsAsync()
        {
            await _permissionRepository.SeedDefaultPermissions();
        }
    }
}
