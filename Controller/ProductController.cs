using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Models;
using DotnetAPI.Repository;
using System.Threading.Tasks;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // ✅ GET all or one product
        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts(
            int? productId = null,
            string searchTerm = null,
            int? categoryId = null,
            int? artistId = null)
        {
            var products = await _productRepository.GetProductsAsync(productId, searchTerm, categoryId, artistId);
            return Ok(products);
        }


        // ✅ POST for insert/update
        [HttpPost("UpsertProduct")]
        public async Task<IActionResult> UpsertProduct([FromBody] Product product)
        {
            if (product == null)
                return BadRequest("Product is null.");

            var result = await _productRepository.UpsertProductAsync(product);

            if (result != null && result.ProductId.HasValue)
                return Ok(result);

            return BadRequest("Failed to process product data.");
        }

        // ✅ PUT for update
        [HttpPut("Update/{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] Product product)
        {
            if (product == null || !product.ProductId.HasValue || product.ProductId.Value != productId)
                return BadRequest("Product ID mismatch or product is null.");

            var updatedProduct = await _productRepository.UpsertProductAsync(product);

            if (updatedProduct != null && updatedProduct.ProductId.HasValue)
                return Ok(updatedProduct);

            return BadRequest("Failed to update product.");
        }

        // ✅ DELETE product
        [HttpDelete("Delete/{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var deleted = await _productRepository.DeleteProductAsync(productId);

            if (deleted)
                return Ok($"Product with ID {productId} deleted successfully.");

            return NotFound($"Product with ID {productId} was not found or already deleted.");
        }

        // // ✅ SEARCH with optional filters
        // [HttpGet("Search")]
        // public async Task<IActionResult> SearchProducts(string searchTerm = null, int? categoryId = null, int? artistId = null)
        // {
        //     var results = await _productRepository.SearchProductsAsync(searchTerm, categoryId, artistId);
        //     return Ok(results);
        // }
    }
}
