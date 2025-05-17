using portal_agile.Dtos.Users;
using portal_agile.Models;

namespace portal_agile.Contracts.Repositories
{
    public interface IUserRepository : IBaseRepository<User, string>
    {
        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<User> GetUserByEmail(string email);

        /// <summary>
        /// Get user by phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        Task<User> GetUserByPhoneNumber(string phoneNumber);

        /// <summary>
        /// Get user by user name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<User> GetUserByUserName(string userName);

        /// <summary>
        /// Deactiviate user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<User> DeactivateUser(string userId);
    }
}
