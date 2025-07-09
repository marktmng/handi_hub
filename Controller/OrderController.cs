using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Models;
using DotnetAPI.Repository;
using DotnetAPI.Data;
using System.Threading.Tasks;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // Orders
        [HttpGet("GetOrders")]
        public async Task<IActionResult> GetOrders(int? orderId = null)
        {
            var orders = await _orderRepository.GetOrdersAsync(orderId);
            return Ok(orders);
        }

        [HttpPost("UpsertOrder")]
        public async Task<IActionResult> UpsertOrder([FromBody] Order order)
        {
            if (order == null)
                return BadRequest("Order is null.");

            if (order.TotalAmount < 0)
                return BadRequest("TotalAmount cannot be negative.");

            var result = await _orderRepository.UpsertOrderAsync(order);
            if (result != null && result.OrderId.HasValue)
                return Ok(result);

            return BadRequest("Failed to process order data.");
        }

        [HttpPut("Update/{orderId}")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] Order order)
        {
            if (order == null || !order.OrderId.HasValue || order.OrderId.Value != orderId)
                return BadRequest("Order ID mismatch or order is null.");

            if (order.TotalAmount < 0)
                return BadRequest("TotalAmount cannot be negative.");

            var updatedOrder = await _orderRepository.UpsertOrderAsync(order);

            if (updatedOrder != null && updatedOrder.OrderId.HasValue)
                return Ok(updatedOrder);

            return BadRequest("Failed to update order.");
        }

        [HttpDelete("Delete/{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var deleted = await _orderRepository.DeleteOrderAsync(orderId);
            if (deleted)
                return Ok($"Order with ID {orderId} deleted successfully.");

            return NotFound($"Order with ID {orderId} was not found or already deleted.");
        }

        // OrderItems
        [HttpGet("{orderId}/Items")]
        public async Task<IActionResult> GetOrderItems(int orderId)
        {
            var items = await _orderRepository.GetOrderItemsByOrderIdAsync(orderId);
            return Ok(items);
        }

        [HttpPost("{orderId}/Items")]
        public async Task<IActionResult> UpsertOrderItem(int orderId, [FromBody] OrderItem orderItem)
        {
            if (orderItem == null)
                return BadRequest("OrderItem is null.");

            if (orderItem.OrderId != orderId)
                return BadRequest("OrderId mismatch.");

            if (orderItem.Quantity <= 0)
                return BadRequest("Quantity must be greater than 0.");

            if (orderItem.Price < 0)
                return BadRequest("Price cannot be negative.");

            var result = await _orderRepository.UpsertOrderItemAsync(orderItem);
            if (result != null && result.OrderItemId.HasValue)
                return Ok(result);

            return BadRequest("Failed to process order item data.");
        }

        [HttpDelete("Items/Delete/{orderItemId}")]
        public async Task<IActionResult> DeleteOrderItem(int orderItemId)
        {
            var deleted = await _orderRepository.DeleteOrderItemAsync(orderItemId);
            if (deleted)
                return Ok($"OrderItem with ID {orderItemId} deleted successfully.");

            return NotFound($"OrderItem with ID {orderItemId} was not found or already deleted.");
        }
    }
}
