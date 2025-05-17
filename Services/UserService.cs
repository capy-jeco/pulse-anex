using AutoMapper;
using Microsoft.AspNetCore.Identity;
using portal_agile.Contracts.Repositories;
using portal_agile.Contracts.Services;
using portal_agile.Dtos.Users;
using portal_agile.Exceptions.Users;
using portal_agile.Models;
using portal_agile.Security;

namespace portal_agile.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IMapper mapper,
            UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAll();
            if (!users.Any())
            {
                throw new UserNotFoundException("No users found");
            }
            var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);
            return usersDto;
        }

        /// <inheritdoc/>
        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userRepository.GetById(userId);
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }
            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        /// <inheritdoc/>
        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new UserNotFoundException($"User with email:{email} not found.");
            }
            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        /// <inheritdoc/>
        public async Task<UserDto> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            var user = await _userRepository.GetUserByPhoneNumber(phoneNumber);
            if (user == null)
            {
                throw new UserNotFoundException($"User with phone number:{phoneNumber} not found.");
            }
            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        /// <inheritdoc/>
        public async Task<UserDto> GetUserByUserNameAsync(string userName)
        {
            var user = await _userRepository.GetUserByUserName(userName);
            if (user == null)
            {
                throw new UserNotFoundException($"User with user name:{userName} not found.");
            }
            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        public async Task AssignToRoleAsync(string userId, string roleName)
        {
            var user = await _userRepository.GetById(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var role = await _roleRepository.GetRoleByName(roleName);
            if (role == null)
                throw new KeyNotFoundException("Role not found");

            if (!await _roleManager.RoleExistsAsync(roleName))
                throw new Exception("Role does not exist");

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
                throw new Exception("Failed to assign role");
        }

        public async Task AssignPermissionAsync(string userId, string permissionName)
        {
            
        }


        /// <inheritdoc/>
        public async Task<UserDto> DeactivateUserAsync(string userId)
        {
            var user = await _userRepository.DeactivateUser(userId);
            if (user == null)
            {
                throw new UserNotFoundException($"User with ID {userId} not found.");
            }
            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }
    }
}
