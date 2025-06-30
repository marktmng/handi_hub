using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Models;
using DotnetAPI.Repository;
using DotnetAPI.Data;
using System.Threading.Tasks;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // Get all orders or a specific order by ID
        [HttpGet("GetOrders")]
        public async Task<IActionResult> GetOrders(int? orderId = null)
        {
            var orders = await _orderRepository.GetOrdersAsync(orderId);
            return Ok(orders);
        }

        // Create a new order
        [HttpPost("UpsertOrder")]
        public async Task<IActionResult> UpsertOrder([FromBody] Order order)
        {
            if (order == null)
                return BadRequest("Order is null.");

            if (order.TotalAmount <= 0)
                return BadRequest("TotalAmount must be greater than 0.");

            var result = await _orderRepository.UpsertOrderAsync(order);

            if (result != null && result.OrderId.HasValue)
                return Ok(result);

            return BadRequest("Failed to process order data.");
        }

        // Update an existing order
        [HttpPut("Update/{orderId}")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] Order order)
        {
            if (order == null || !order.OrderId.HasValue || order.OrderId.Value != orderId)
                return BadRequest("Order ID mismatch or order is null.");

            if (order.TotalAmount <= 0)
                return BadRequest("TotalAmount must be greater than 0.");

            var updatedOrder = await _orderRepository.UpsertOrderAsync(order);

            if (updatedOrder != null && updatedOrder.OrderId.HasValue)
                return Ok(updatedOrder);

            return BadRequest("Failed to update order.");
        }

        // Delete an order
        [HttpDelete("Delete/{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var deleted = await _orderRepository.DeleteOrderAsync(orderId);

            if (deleted)
                return Ok($"Order with ID {orderId} deleted successfully.");

            return NotFound($"Order with ID {orderId} was not found or already deleted.");
        }
    }
}
