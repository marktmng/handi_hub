using DotnetAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetAPI.Data
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsersAsync(int? userId = null, string role = null);
        Task<User> UpsertUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> UpdateUserRoleAsync(int userId, string role);
    }
}
