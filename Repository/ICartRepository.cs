using DotnetAPI.Models;
using DotnetAPI.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetAPI.Data
{
    public interface ICartRepository
    {
        Task<IEnumerable<CartItemDto>> GetCartItemsByUserIdAsync(int userId);
        Task<CartItem> UpsertCartItemAsync(CartItem cartItem);
        Task<bool> DeleteCartItemAsync(int cartId);
        Task<bool> ClearCartByUserIdAsync(int userId);
    }
}