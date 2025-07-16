using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace DotnetAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;

        public ProductRepository(DataContext context)
        {
            _context = context;
        }

        // ✅ Get products with optional filter by ProductId
        public async Task<IEnumerable<ProductReadOnlyDto>> GetProductsAsync(
            int? productId = null,
            string searchTerm = null,
            int? categoryId = null,
            int? artistId = null)
        {
            var parameters = new[]
            {
        new SqlParameter("@ProductId", (object?)productId ?? DBNull.Value),
        new SqlParameter("@SearchTerm", (object?)searchTerm ?? DBNull.Value),
        new SqlParameter("@CategoryId", (object?)categoryId ?? DBNull.Value),
        new SqlParameter("@ArtistId", (object?)artistId ?? DBNull.Value)
    };

            return await _context.ProductReadOnlyDtos
                .FromSqlRaw("EXEC HandiHub.spProducts_Get @ProductId, @SearchTerm, @CategoryId, @ArtistId", parameters)
                .ToListAsync();
        }


        // ✅ Insert or update product
        public async Task<Product> UpsertProductAsync(Product product)
        {
            var productIdParam = new SqlParameter("@ProductId", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.InputOutput,
                Value = product.ProductId.HasValue && product.ProductId.Value != 0 ? product.ProductId.Value : (object)DBNull.Value
            };

            var parameters = new[]
            {
                productIdParam,
                new SqlParameter("@ProductName", product.ProductName ?? (object)DBNull.Value),
                new SqlParameter("@CategoryId", product.CategoryId),
                new SqlParameter("@ProductDesc", product.ProductDesc ?? (object)DBNull.Value),
                new SqlParameter("@ProductImage", product.ProductImage ?? (object)DBNull.Value),
                new SqlParameter("@ActualPrice", product.ActualPrice),
                new SqlParameter("@SellingPrice", product.SellingPrice),
                new SqlParameter("@Quantity", product.Quantity),
                new SqlParameter("@ArtistId", product.ArtistId)
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC HandiHub.spProducts_Upsert @ProductId OUTPUT, @ProductName, @CategoryId, @ProductDesc, @ProductImage, @ActualPrice, @SellingPrice, @Quantity, @ArtistId",
                parameters);

            product.ProductId = (int?)productIdParam.Value;
            return product;
        }

        // ✅ Delete product by ID
        public async Task<bool> DeleteProductAsync(int productId)
        {
            var productIdParam = new SqlParameter("@ProductId", productId);
            var rowsAffectedParam = new SqlParameter("@RowsAffected", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC HandiHub.spProducts_Delete @ProductId, @RowsAffected OUTPUT",
                productIdParam, rowsAffectedParam);

            return (int)rowsAffectedParam.Value > 0;
        }

        // ✅ Search product by keyword, category, or artist
        public async Task<IEnumerable<ProductReadOnlyDto>> SearchProductsAsync(string searchTerm = null, int? categoryId = null, int? artistId = null)
        {
            var searchTermParam = new SqlParameter("@SearchTerm", (object?)searchTerm ?? DBNull.Value);
            var categoryIdParam = new SqlParameter("@CategoryId", (object?)categoryId ?? DBNull.Value);
            var artistIdParam = new SqlParameter("@ArtistId", (object?)artistId ?? DBNull.Value);

            return await _context.ProductReadOnlyDtos
                .FromSqlRaw("EXEC HandiHub.spProducts_Search @SearchTerm, @CategoryId, @ArtistId",
                    searchTermParam, categoryIdParam, artistIdParam)
                .ToListAsync();
        }
    }
}
