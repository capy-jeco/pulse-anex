﻿using System.Security.Claims;
using System.Threading.Tasks;
using portal_agile.Dtos.Permissions;
using portal_agile.Dtos.Users;
using portal_agile.Models;
using portal_agile.Security;

namespace portal_agile.Contracts.Repositories
{
    public interface IUserRepository : IBaseRepository<User, string>
    {
        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<User?> GetUserByEmail(string email);

        /// <summary>
        /// Get user by phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        Task<User?> GetUserByPhoneNumber(string phoneNumber);

        /// <summary>
        /// Get user by user name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<User?> GetUserByUserName(string userName);

        /// <summary>
        /// Get roles of a user by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<Role>?> GetRolesByUserId(string userId);

        /// <summary>
        /// Get all permissions of a user by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<Permission>?> GetAllUserPermissionsByUserId(string userId);

        /// <summary>
        /// Get user permission claims
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IList<Claim>> GetUserPermissionClaimsByUserId(string userId);

        /// <summary>
        /// Get user by refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>A user</returns>
        Task<User?> GetByRefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Assign a role to user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<bool> AssignRoleToUserByRoleName(string userId, string roleName);

        /// <summary>
        /// Deactiviate user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> DeactivateUserByUserId(string userId);
    }
}
