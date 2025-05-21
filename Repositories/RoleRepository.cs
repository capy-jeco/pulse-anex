using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using portal_agile.Contracts.Repositories;
using portal_agile.Data;
using portal_agile.Models;
using portal_agile.Security;

namespace portal_agile.Repositories
{
    public class RoleRepository : BaseRepository<Role, string, AppDbContext>, IRoleRepository
    {
        private readonly RoleManager<Role> _roleManager;
        public RoleRepository(
            AppDbContext context,
            RoleManager<Role> roleManager)
            : base(context)
        {
            _roleManager = roleManager;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Role>> GetAllRoles()
        {
            var roles = await base.GetAll();

            return roles;
        }

        /// <inheritdoc/>
        public async Task<Role?> GetRoleByName(string roleName)
        {
            var roleResult = await base.Search(_dbSet, roleName);

            var role = roleResult.Where(r => r.Name == roleName).FirstOrDefault();

            return role;
        }

        /// <inheritdoc/>
        public async Task<List<Permission>?> GetRolePermissions(string roleId)
        {
            var role = await GetById(roleId);
            if (role == null)
            {
                throw new ArgumentException($"Role with ID {roleId} not found.");
            }

            var permissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission)
                .Where(p => p.IsActive)
                .ToListAsync();

            return permissions;
        }
    }
}
