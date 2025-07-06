using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace DotnetAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly DataContext _context;

        public CartRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartItemDto>> GetCartItemsByUserIdAsync(int userId)
        {
            var userIdParam = new SqlParameter("@UserId", userId);
            return await _context.CartItemDtos
                .FromSqlRaw("EXEC HandiHub.spCartItems_GetByUserId @UserId", userIdParam)
                .ToListAsync();
        }

        public async Task<CartItem> UpsertCartItemAsync(CartItem cartItem)
        {
            var sellingPrice = await _context.Products
                .Where(p => p.ProductId == cartItem.ProductId)
                .Select(p => p.SellingPrice)
                .FirstOrDefaultAsync();

            cartItem.Price = sellingPrice;

            var cartIdParam = new SqlParameter("@CartId", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.InputOutput,
                Value = cartItem.CartId ?? (object)DBNull.Value
            };

            var parameters = new[]
            {
                new SqlParameter("@UserId", cartItem.UserId),
                new SqlParameter("@ProductId", cartItem.ProductId),
                new SqlParameter("@Quantity", cartItem.Quantity),
                new SqlParameter("@Price", cartItem.Price),
                cartIdParam
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC HandiHub.spCartItems_Upsert @UserId, @ProductId, @Quantity, @Price, @CartId OUTPUT",
                parameters
            );

            cartItem.CartId = (int?)cartIdParam.Value;
            return cartItem;
        }

        public async Task<bool> DeleteCartItemAsync(int cartId)
        {
            var cartIdParam = new SqlParameter("@CartId", cartId);
            var rowsAffectedParam = new SqlParameter("@RowsAffected", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC HandiHub.spCartItems_Delete @CartId, @RowsAffected OUTPUT",
                cartIdParam, rowsAffectedParam
            );

            return (int)rowsAffectedParam.Value > 0;
        }

        public async Task<bool> ClearCartByUserIdAsync(int userId)
        {
            var userIdParam = new SqlParameter("@UserId", userId);

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC HandiHub.spCartItems_ClearByUserId @UserId",
                userIdParam
            );

            return true;
        }
    }
}