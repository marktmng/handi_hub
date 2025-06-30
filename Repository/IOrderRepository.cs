using DotnetAPI.Models;
using DotnetAPI.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetAPI.Data
{
    public interface IOrderRepository
    {
        // Orders
        Task<IEnumerable<OrderDto>> GetOrdersAsync(int? orderId = null);
        Task<Order> UpsertOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int orderId);

        // OrderItems
        Task<IEnumerable<OrderItemDto>> GetOrderItemsByOrderIdAsync(int orderId);
        Task<OrderItem> UpsertOrderItemAsync(OrderItem orderItem);
        Task<bool> DeleteOrderItemAsync(int orderItemId);

        // Calculation
        Task<decimal> CalculateOrderTotalAsync(int orderId);

        // Product Pricing
        Task<decimal> GetProductSellingPriceAsync(int productId);
    }
}