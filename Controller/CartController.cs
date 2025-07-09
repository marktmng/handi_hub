using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Models;
using DotnetAPI.Repository;
using DotnetAPI.Data;
using System.Threading.Tasks;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        [HttpGet("{userId}/Items")]
        public async Task<IActionResult> GetCartItems(int userId)
        {
            var items = await _cartRepository.GetCartItemsByUserIdAsync(userId);
            return Ok(items);
        }

        [HttpPost("{userId}/Items")]
        public async Task<IActionResult> UpsertCartItem(int userId, [FromBody] CartItem cartItem)
        {
            if (cartItem == null)
                return BadRequest("CartItem is null.");

            if (cartItem.UserId != userId)
                return BadRequest("UserId mismatch.");

            if (cartItem.Quantity <= 0)
                return BadRequest("Quantity must be greater than 0.");

            var result = await _cartRepository.UpsertCartItemAsync(cartItem);
            return Ok(result);
        }

        [HttpDelete("Items/Delete/{cartId}")]
        public async Task<IActionResult> DeleteCartItem(int cartId)
        {
            var deleted = await _cartRepository.DeleteCartItemAsync(cartId);
            if (deleted)
                return Ok($"CartItem with ID {cartId} deleted successfully.");

            return NotFound($"CartItem with ID {cartId} was not found or already deleted.");
        }

        [HttpDelete("Clear/{userId}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            await _cartRepository.ClearCartByUserIdAsync(userId);
            return Ok($"Cart cleared for user ID {userId}.");
        }
    }
}
