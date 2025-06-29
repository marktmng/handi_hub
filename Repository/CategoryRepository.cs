using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotnetAPI.Dtos; // Assuming UsersDto is defined in Dtos namespace

namespace DotnetAPI.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context; // DataContext to access the database

        public CategoryRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(int? categoryId = null)
        {
            var parameters = new List<SqlParameter>();

            var categoryIdParam = new SqlParameter("@CategoryId", categoryId.HasValue ? categoryId.Value : (object)DBNull.Value);

            parameters.Add(categoryIdParam);

            string sql = "EXEC HandiHub.spCategories_Get @CategoryId";

            return await _context.CategoryDtos
                .FromSqlRaw(sql, parameters.ToArray())
                .ToListAsync();
        }

        public async Task<Category> UpsertCategoryAsync(Category category)
        {
            var categoryIdParam = new SqlParameter("@CategoryId", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.InputOutput,
                Value = category.CategoryId.HasValue && category.CategoryId.Value != 0 ? category.CategoryId.Value : (object)DBNull.Value
            };

            var parameters = new[]
            {
                categoryIdParam,
                new SqlParameter("@CategoryName", category.CategoryName ?? (object)DBNull.Value)

        };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC HandiHub.spCategories_Upsert @CategoryId OUTPUT, @CategoryName",
                parameters);

            category.CategoryId = (int?)categoryIdParam.Value;
            return category;
        }


        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var categoryIdParam = new SqlParameter("@CategoryId", categoryId);
            var rowsAffectedParam = new SqlParameter("@RowsAffected", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC HandiHub.spCategories_Delete @CategoryId, @RowsAffected OUTPUT",
                categoryIdParam, rowsAffectedParam);

            int rowsDeleted = (int)rowsAffectedParam.Value;
            Console.WriteLine($"DeleteCategoryAsync affected rows: {rowsDeleted}");

            return rowsDeleted > 0;
        }
    }
}
