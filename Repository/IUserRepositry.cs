using DotnetAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using DotnetAPI.Dtos;

namespace DotnetAPI.Data
{
    public interface IUserRepository
    {
        Task<IEnumerable<UsersDto>> GetUsersAsync(int? userId = null, string role = null);
        Task<User> UpsertUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> UpdateUserRoleAsync(int userId, string role);
    }
}
