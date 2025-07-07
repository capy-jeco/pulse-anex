using System.Security.Claims;
using AutoMapper;
using portal_agile.Contracts.Repositories;
using portal_agile.Contracts.Services;
using portal_agile.Dtos.Permissions;
using portal_agile.Exceptions.Users;
using portal_agile.Repositories;
using portal_agile.Security;

namespace portal_agile.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public PermissionService(
            IPermissionRepository permissionRepository,
            IRoleRepository roleRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<List<PermissionDto>> GetAllPermissionsAsync()
        {
            var permissions = await _permissionRepository.GetAll();

            if (permissions == null || !permissions.Any())
            {
                return new List<PermissionDto>();
            }

            return _mapper.Map<List<PermissionDto>>(permissions);
        }

        public async Task<PermissionDto> GetPermissionByIdAsync(int permissionId)
        {
            var permission = await _permissionRepository.GetById(permissionId);

            return _mapper.Map<PermissionDto>(permission);
        }

        /// <inheritdoc/>
        public async Task<List<PermissionDto>> GetRolePermissionsAsync(string roleId)
        {
            var permissions = await _roleRepository.GetRolePermissions(roleId);
            return _mapper.Map<List<PermissionDto>>(permissions);
        }

        /// <inheritdoc/>
        public async Task<PermissionDto> CreateAsync(PermissionCreateDto permissionCreateDto)
        {
            var createdPermission = await _permissionRepository.Create(_mapper.Map<Permission>(permissionCreateDto));
            var permissionDto = _mapper.Map<PermissionDto>(createdPermission);

            return permissionDto;
        }

        /// <inheritdoc/>
        public async Task<PermissionDto> UpdatePermissionAsync(Permission permissionUpdate)
        {
            var permission = await _permissionRepository.GetById(permissionUpdate.PermissionId);
            if (permission == null)
            {
                throw new Exception("Role not found");
            }

            _permissionRepository.Update(permission);
            await _permissionRepository.SaveChangesAsync();

            return _mapper.Map<PermissionDto>(permission);
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
    }
}
