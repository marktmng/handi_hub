using DotnetAPI.Models;
using DotnetAPI.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetAPI.Data
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderDto>> GetOrdersAsync(int? orderId = null);
        Task<Order> UpsertOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int orderId);
    }
}