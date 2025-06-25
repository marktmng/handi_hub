using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context; // DataContext to access the database

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetUsersAsync(int? userId = null, string role = null)
        {
            var parameters = new List<SqlParameter>();

            var userIdParam = new SqlParameter("@UserId", userId.HasValue ? userId.Value : (object)DBNull.Value);
            var roleParam = new SqlParameter("@Role", role ?? (object)DBNull.Value);

            parameters.Add(userIdParam);
            parameters.Add(roleParam);

            string sql = "EXEC HandiHub.spUsers_Get @UserId, @Role";

            return await _context.Users
                .FromSqlRaw(sql, parameters.ToArray())
                .ToListAsync();
        }

        public async Task<User> UpsertUserAsync(User user)
        {
            var userIdParam = new SqlParameter("@UserId", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.InputOutput,
                Value = user.UserId.HasValue && user.UserId.Value != 0 ? user.UserId.Value : (object)DBNull.Value
            };

            var parameters = new[]
            {
                userIdParam,
                new SqlParameter("@FirstName", user.FirstName ?? (object)DBNull.Value),
                new SqlParameter("@LastName", user.LastName ?? (object)DBNull.Value),
                new SqlParameter("@Email", user.Email ?? (object)DBNull.Value),
                new SqlParameter("@UserName", user.UserName ?? (object)DBNull.Value),
                new SqlParameter("@PhoneNumber", user.PhoneNumber ?? (object)DBNull.Value),
                new SqlParameter("@Role", user.Role ?? (object)DBNull.Value)
        };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC HandiHub.spUsers_Upsert @UserId OUTPUT, @FirstName, @LastName, @Email, @UserName, @PhoneNumber, @Role",
                parameters);

            user.UserId = (int?)userIdParam.Value;
            return user;
        }




        public async Task<bool> DeleteUserAsync(int userId)
        {
            var userIdParam = new SqlParameter("@UserId", userId);
            var rowsAffectedParam = new SqlParameter("@RowsAffected", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC HandiHub.spUsers_Delete @UserId, @RowsAffected OUTPUT",
                userIdParam, rowsAffectedParam);

            int rowsDeleted = (int)rowsAffectedParam.Value;
            Console.WriteLine($"DeleteUserAsync affected rows: {rowsDeleted}");

            return rowsDeleted > 0;
        }



        public async Task<bool> UpdateUserRoleAsync(int userId, string role)
        {
            var parameters = new[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Role", role)
            };

            var result = await _context.Database.ExecuteSqlRawAsync("EXEC HandiHub.spUsers_UpdateRole @UserId, @Role", parameters);
            return result > 0;
        }
    }
}
