using System.Linq.Expressions;
using System.Security.Claims;
using portal_agile.Dtos.Permissions;
using portal_agile.Dtos.Roles;
using portal_agile.Dtos.Users;
using portal_agile.Dtos.Users.Requests;
using portal_agile.Models;
using portal_agile.Security;

namespace portal_agile.Contracts.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UserCreateRequest>> GetAllUsersAsync();

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<UserCreateRequest> GetUserByIdAsync(string id);

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<UserCreateRequest> GetUserByEmailAsync(string email);

        /// <summary>
        /// Get user by phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        Task<UserCreateRequest> GetUserByPhoneNumberAsync(string phoneNumber);

        /// <summary>
        /// Get user by user name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<UserCreateRequest> GetUserByUserNameAsync(string userName);

        /// <summary>
        /// Get roles of a user by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<RoleDto>> GetRolesByUserIdAsync(string userId);

        /// <summary>
        /// Get direct permissions of a user by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<PermissionDto>> GetUserDirectPermissionsAsync(string userId);

        /// <summary>
        /// Get all permissions of a user by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<PermissionDto>> GetAllUserPermissionsAsync(string userId);

        /// <summary>
        /// Get all permissions of a user by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>A list of permissions as string</returns>
        Task<List<string>> GetAllUserPermissionsInStringAsync(string userId);

        /// <summary>
        /// Get user permission claims
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IList<Claim>> GetUserPermissionClaimsAsync(string userId);

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="userCreateRequest">New user data</param>
        /// <returns>Newly created user</returns>
        Task<UserDto> Store(UserCreateRequest userCreateRequest);

        /// <summary>
        /// Updates a specific property of an entity identified by its key
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <param name="propertyName">Column name or property to update</param>
        /// <param name="propertyValue">New value for the property</param>
        /// <returns>The updated entity</returns>
        Task<UserDto> UpdateFromKeyAsync(string userId, string propertyName, object propertyValue);

        /// <summary>
        /// Assign a role to user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<bool> AssignRoleToUserAsync(string userId, string roleName);

        /// <summary>
        /// Deactiviate user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> DeactivateUserAsync(string userId);
    }
}
