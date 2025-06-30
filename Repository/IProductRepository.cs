using DotnetAPI.Models;
using DotnetAPI.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetAPI.Data
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync(
            int? productId = null,
            string searchTerm = null,
            int? categoryId = null,
            int? artistId = null);
        Task<Product> UpsertProductAsync(Product product);
        Task<bool> DeleteProductAsync(int productId);
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm = null, int? categoryId = null, int? artistId = null);
    }
}
