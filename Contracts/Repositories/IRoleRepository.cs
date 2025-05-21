using portal_agile.Security;

namespace portal_agile.Contracts.Repositories
{
    public interface IRoleRepository : IBaseRepository<Role, string>
    {
        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns>List of all roles</returns>
        Task<IEnumerable<Role>> GetAllRoles();

        /// <summary>
        /// Get role by name
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<Role?> GetRoleByName(string roleName);

        /// <summary>
        /// Get permissions of role by role id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<List<Permission>?> GetRolePermissions(string roleId);
    }
}
