using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Data;
using DotnetAPI.Models;
using System.Threading.Tasks;
using DotnetAPI.Repository;
using Microsoft.AspNetCore.Authorization;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository; // Injected repository to access catery data

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [AllowAnonymous]
        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories(int? categoryId = null)
        {
            var categories = await _categoryRepository.GetCategoriesAsync(categoryId);
            return Ok(categories);
        }

        // [Authorize]
        [HttpPost("UpsertCategory")]
        public async Task<IActionResult> UpsertCategory([FromBody] Category category)
        {
            if (category == null)
                return BadRequest("Category is null.");

            var result = await _categoryRepository.UpsertCategoryAsync(category);
            if (result != null && result.CategoryId.HasValue)
                return Ok(result);

            return BadRequest("Failed to process category data.");
        }

        // New PUT endpoint for updating the full category info
        // [Authorize]
        [HttpPut("Update/{categoryId}")]
        public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] Category category)
        {
            if (category == null || category.CategoryId != categoryId)
                return BadRequest("Category ID mismatch or category is null.");

            var updatedUser = await _categoryRepository.UpsertCategoryAsync(category);
            if (updatedUser != null && updatedUser.CategoryId.HasValue)
                return Ok(updatedUser);

            return BadRequest("Failed to update category.");
        }

        // [Authorize]
        [HttpDelete("DeleteUser/{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var deleted = await _categoryRepository.DeleteCategoryAsync(categoryId);

            if (deleted)
                return Ok($"Category with ID {categoryId} deleted successfully.");

            return NotFound($"Category with ID {categoryId} was not found or already deleted.");
        }
    }
}
