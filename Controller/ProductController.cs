using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Models;
using DotnetAPI.Repository;
using System.Threading.Tasks;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _environment;

        public ProductController(IProductRepository productRepository, IWebHostEnvironment environment)

        {
            _productRepository = productRepository;
            _environment = environment;
        }

        // ✅ GET all or one product
        [AllowAnonymous]
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
        // [Authorize]
        [HttpPost("UpsertProduct")]
        public async Task<IActionResult> UpsertProduct([FromForm] ProductDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid product data.");

            string imageUrl = dto.ProductImage; // keep existing image URL if no new image uploaded

            if (dto.ProductImageFile != null && dto.ProductImageFile.Length > 0)
            {
                // Defensive fallback if WebRootPath is null
                string webRootPath = _environment.WebRootPath;
                if (string.IsNullOrEmpty(webRootPath))
                {
                    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }

                var uploadsFolder = Path.Combine(webRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ProductImageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ProductImageFile.CopyToAsync(stream);
                }

                imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
            }

            var product = new Product
            {
                ProductId = dto.ProductId,
                ProductName = dto.ProductName,
                CategoryId = dto.CategoryId,
                ProductDesc = dto.ProductDesc,
                ProductImage = imageUrl,
                ActualPrice = dto.ActualPrice,
                SellingPrice = dto.SellingPrice,
                Quantity = dto.Quantity,
                ArtistId = dto.ArtistId
            };

            var savedProduct = await _productRepository.UpsertProductAsync(product);

            if (savedProduct != null && savedProduct.ProductId.HasValue)
                return Ok(savedProduct);

            return BadRequest("Failed to save product.");
        }


        // ✅ PUT for update
        // [Authorize]
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
        // [Authorize]
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
