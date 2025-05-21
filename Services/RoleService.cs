using AutoMapper;
using portal_agile.Contracts.Repositories;
using portal_agile.Contracts.Services;
using portal_agile.Dtos.Permissions;
using portal_agile.Dtos.Roles;
using portal_agile.Security;

namespace portal_agile.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        public RoleService(
            IRoleRepository roleRepository, 
            IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAll();
            var roleDto = _mapper.Map<IEnumerable<RoleDto>>(roles);
            return roleDto;
        }

        /// <inheritdoc/>
        public async Task<RoleDto> GetRoleByIdAsync(string roleId)
        {
            var role = await _roleRepository.GetById(roleId);
            var roleDto = _mapper.Map<RoleDto>(role);
            return roleDto;
        }

        /// <inheritdoc/>
        public async Task<List<PermissionDto>?> GetRolePermissionsAsync(string roleId)
        {
            var permissions = await _roleRepository.GetRolePermissions(roleId)
                ?? throw new Exception("Role not found");
            var permissionDto = _mapper.Map<List<PermissionDto>>(permissions);
            return permissionDto;
        }


        /// <inheritdoc/>
        public async Task<RoleDto> CreateRoleAsync(RoleCreateDto roleCreateDto)
        {
            var role = _mapper.Map<Role>(roleCreateDto);
            var createdRole = await _roleRepository.Create(role);

            if (createdRole == null)
            {
                throw new Exception("Role creation failed");
            }

            var roleDto = _mapper.Map<RoleDto>(createdRole);
            return roleDto;
        }

        /// <inheritdoc/>
        public async Task<RoleDto> UpdateRoleAsync(string roleId, RoleDto roleUpdate)
        {
            var role = await _roleRepository.GetById(roleUpdate.Id);
            if (role == null)
            {
                throw new Exception("Role not found");
            }

            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();

            var roleDto = _mapper.Map<RoleDto>(role);

            return roleDto;
        }

        /// <inheritdoc/>
        public async Task<bool> SoftDeleteRoleAsync(string roleId)
        {
            var role = await _roleRepository.GetById(roleId);
            if (role == null)
            {
                throw new Exception("Role not found");
            }
            
            role.IsDeleted = true;
            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();
            
            return role.IsDeleted;
        }

        /// <inheritdoc/>
        public async Task<bool> RoleExistsAsync(string roleId)
        {
            var role = await _roleRepository.GetById(roleId);
            return role != null;
        }
    }
}
