using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using portal_agile.Contracts.Repositories;
using portal_agile.Contracts.Services;
using portal_agile.Dtos.Permissions;
using portal_agile.Dtos.Roles;
using portal_agile.Dtos.Users;
using portal_agile.Dtos.Users.Requests;
using portal_agile.Exceptions.Users;
using portal_agile.Helpers;
using portal_agile.Models;
using portal_agile.Repositories;
using portal_agile.Security;

namespace portal_agile.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        private readonly IInputValidator _inputValidator;

        private readonly RoleManager<Role> _roleManager;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IMapper mapper,
            IInputValidator inputValidator,
            UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _mapper = mapper;
            _inputValidator = inputValidator;
            _roleManager = roleManager;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserCreateRequest>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAll();
            if (!users.Any())
            {
                throw new UserNotFoundException("No users found");
            }
            var usersDto = _mapper.Map<IEnumerable<UserCreateRequest>>(users);
            return usersDto;
        }

        /// <inheritdoc/>
        public async Task<UserCreateRequest> GetUserByIdAsync(string userId)
        {
            var user = await _userRepository.GetById(userId);
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }
            var userDto = _mapper.Map<UserCreateRequest>(user);
            return userDto;
        }

        /// <inheritdoc/>
        public async Task<UserCreateRequest> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new UserNotFoundException($"User with email:{email} not found.");
            }
            var userDto = _mapper.Map<UserCreateRequest>(user);
            return userDto;
        }

        /// <inheritdoc/>
        public async Task<UserCreateRequest> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            var user = await _userRepository.GetUserByPhoneNumber(phoneNumber);
            if (user == null)
            {
                throw new UserNotFoundException($"User with phone number:{phoneNumber} not found.");
            }
            var userDto = _mapper.Map<UserCreateRequest>(user);
            return userDto;
        }

        /// <inheritdoc/>
        public async Task<UserCreateRequest> GetUserByUserNameAsync(string userName)
        {
            var user = await _userRepository.GetUserByUserName(userName);
            if (user == null)
            {
                throw new UserNotFoundException($"User with user name:{userName} not found.");
            }
            var userDto = _mapper.Map<UserCreateRequest>(user);
            return userDto;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<RoleDto>> GetRolesByUserIdAsync(string userId)
        {
            var user = await _userRepository.GetById(userId) ?? throw new UserNotFoundException(userId);

            return _mapper.Map<IEnumerable<RoleDto>>(await _userRepository.GetRolesByUserId(userId));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PermissionDto>> GetUserDirectPermissionsAsync(string userId)
        {
            var permissions = await _userRepository.GetUserPermissionClaimsByUserId(userId);
            return _mapper.Map<List<PermissionDto>>(permissions);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PermissionDto>> GetAllUserPermissionsAsync(string userId)
        {
            if (!_inputValidator.IsValidGuid(userId, out var normalizedGuid))
            {
                throw new InvalidUserIdException("User ID is not valid");
            }

            var permissions = await _userRepository.GetAllUserPermissionsByUserId(userId);
            return _mapper.Map<List<PermissionDto>>(permissions);
        }

        /// <inheritdoc/>
        public async Task<IList<Claim>> GetUserPermissionClaimsAsync(string userId)
        {
            if (!_inputValidator.IsValidGuid(userId, out var normalizedGuid))
            {
                throw new InvalidUserIdException("User ID is not valid");
            }

            var user = await _userRepository.GetById(userId)
                ?? throw new UserNotFoundException(userId);

            var permissionClaims = await _userRepository.GetUserPermissionClaimsByUserId(userId);
            if (permissionClaims == null || !permissionClaims.Any())
            {
                return new List<Claim>();
            }

            return permissionClaims;
        }

        public async Task<UserDto> Store(UserCreateRequest userCreateRequest)
        {
            var user = _mapper.Map<User>(userCreateRequest);
            var createdUser = await _userRepository.Create(user);

            if (createdUser == null)
            {
                throw new InvalidUserCreateDataException("User creation failed. Invalid user data.");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateFromKeyAsync(string userId, string propertyName, object newValue)
        {
            if (!_inputValidator.IsValidGuid(userId, out var normalizedGuid))
            {
                throw new InvalidUserIdException("User ID is not valid");
            }

            var updatedUser = await _userRepository.UpdateFromKey(userId, propertyName, newValue);

            return _mapper.Map<UserDto>(updatedUser);
        }

        /// <inheritdoc/>
        public async Task<bool> AssignRoleToUserAsync(string userId, string roleName)
        {
            if (!_inputValidator.IsValidGuid(userId, out var normalizedGuid))
            {
                throw new InvalidUserIdException("User ID is not valid");
            }

            var user = await _userRepository.GetById(userId) 
                ?? throw new UserNotFoundException("User not found");

            if (!await _roleManager.RoleExistsAsync(roleName))
                throw new Exception($"Role with role name: {roleName}, not found.");

            return await _userRepository.AssignRoleToUserByRoleName(userId, roleName);
        }

        public async Task<bool> AssignDirectPermissionsToUserAsync(string userId, IEnumerable<int> permissionIds, string modifiedBy)
        {
            if (!_inputValidator.IsValidGuid(userId, out var normalizedGuid))
            {
                throw new InvalidUserIdException("User ID is not valid");
            }

            var user = await _userRepository.GetById(userId)
                ?? throw new UserNotFoundException(userId);

            var permissionsAreAssigned = await _userRepository.AssignUserDirectPermissions(userId, permissionIds, modifiedBy);

            return permissionsAreAssigned;
        }

        public async Task<bool> AssignPermissionToUserAsync(string userId, int permissionId, string modifiedBy)
        {
            if (!_inputValidator.IsValidGuid(userId, out var normalizedGuid))
            {
                throw new InvalidUserIdException("User ID is not valid");
            }

            var user = await _userRepository.GetById(userId);
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }

            var permission = await _permissionRepository.GetById(permissionId);
            if (permission == null)
            {
                throw new Exception("Permission not found");
            }

            var permissionsAreAssigned = await _userRepository.AssignPermissionToUser(userId, permissionId, modifiedBy);

            return permissionsAreAssigned;
        }

        public async Task<IEnumerable<PermissionDto>> RevokeDirectPermissionsFromUserAsync(string userId, IEnumerable<int> permissionIds, string modifiedBy)
        {
            IEnumerable<PermissionDto> updatedPermissions = [];

            if (!_inputValidator.IsValidGuid(userId, out var normalizedGuid))
            {
                throw new InvalidUserIdException("User ID is not valid");
            }

            var user = await _userRepository.GetById(userId)
                ?? throw new UserNotFoundException(userId);

            var result = await _userRepository.RevokeDirectPermissionsFromUser(userId, permissionIds, modifiedBy);

            if (result)
            {
                updatedPermissions = _mapper.Map<IEnumerable<PermissionDto>>(await _userRepository.GetAllUserPermissionsByUserId(userId));
            }

            return updatedPermissions;
        }


        /// <inheritdoc/>
        public async Task<bool> DeactivateUserAsync(string userId)
        {
            if (!_inputValidator.IsValidGuid(userId, out var normalizedGuid))
            {
                throw new InvalidUserIdException("User ID is not valid");
            }

            var user = await _userRepository.GetById(userId)
                ?? throw new UserNotFoundException($"User with ID {userId} not found.");

            var deactivated = await _userRepository.DeactivateUserByUserId(userId);

            if (!deactivated)
            {
                throw new Exception($"Failed to deactivate user with ID {userId}.");
            }

            return deactivated;
        }
    }
}
