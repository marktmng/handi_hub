using DotnetAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using DotnetAPI.Dtos;

namespace DotnetAPI.Data
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync(int? userId = null);
        Task<Category> UpsertCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int categoryId);
    }
}
