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
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _context;

        public OrderRepository(DataContext context)
        {
            _context = context;
        }

        // Orders
        public async Task<IEnumerable<OrderDto>> GetOrdersAsync(int? orderId = null)
        {
            var orderIdParam = new SqlParameter("@OrderId", (object?)orderId ?? DBNull.Value);

            return await _context.OrderDtos
                .FromSqlRaw("EXEC HandiHub.spOrders_Get @OrderId", orderIdParam)
                .ToListAsync();
        }

        public async Task<Order> UpsertOrderAsync(Order order)
        {
            var orderIdParam = new SqlParameter("@OrderId", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.InputOutput,
                Value = order.OrderId ?? (object)DBNull.Value
            };

            var parameters = new[]
            {
                orderIdParam,
                new SqlParameter("@UserId", order.UserId),
                new SqlParameter("@TotalAmount", order.TotalAmount)
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC HandiHub.spOrders_Upsert @OrderId OUTPUT, @UserId, @TotalAmount", parameters);

            order.OrderId = (int?)orderIdParam.Value;
            return order;
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var orderIdParam = new SqlParameter("@OrderId", orderId);
            var rowsAffectedParam = new SqlParameter("@RowsAffected", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC HandiHub.spOrders_Delete @OrderId, @RowsAffected OUTPUT", orderIdParam, rowsAffectedParam);

            return (int)rowsAffectedParam.Value > 0;
        }

        // OrderItems
        public async Task<IEnumerable<OrderItemDto>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            var orderIdParam = new SqlParameter("@OrderId", orderId);

            return await _context.OrderItemDtos
                .FromSqlRaw("EXEC HandiHub.spOrderItems_GetByOrderId @OrderId", orderIdParam)
                .ToListAsync();
        }

        public async Task<OrderItem> UpsertOrderItemAsync(OrderItem orderItem)
        {
            var sellingPrice = await GetProductSellingPriceAsync(orderItem.ProductId);
            orderItem.Price = orderItem.Quantity * sellingPrice;

            var orderItemIdParam = new SqlParameter("@OrderItemId", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.InputOutput,
                Value = orderItem.OrderItemId ?? (object)DBNull.Value
            };

            var parameters = new[]
            {
                orderItemIdParam,
                new SqlParameter("@OrderId", orderItem.OrderId),
                new SqlParameter("@ProductId", orderItem.ProductId),
                new SqlParameter("@Quantity", orderItem.Quantity),
                new SqlParameter("@Price", orderItem.Price)
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC HandiHub.spOrderItems_Upsert @OrderItemId OUTPUT, @OrderId, @ProductId, @Quantity, @Price", parameters);

            orderItem.OrderItemId = (int?)orderItemIdParam.Value;

            // Update total amount of the order after inserting/updating an item
            decimal total = await CalculateOrderTotalAsync(orderItem.OrderId);
            var orderToUpdate = await _context.Orders.FindAsync(orderItem.OrderId);
            if (orderToUpdate != null)
            {
                orderToUpdate.TotalAmount = total;
                await UpsertOrderAsync(orderToUpdate);
            }

            return orderItem;
        }

        public async Task<bool> DeleteOrderItemAsync(int orderItemId)
        {
            var orderItem = await _context.OrderItems.FindAsync(orderItemId);
            if (orderItem == null)
                return false;

            int orderId = orderItem.OrderId;

            var orderItemIdParam = new SqlParameter("@OrderItemId", orderItemId);
            var rowsAffectedParam = new SqlParameter("@RowsAffected", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC HandiHub.spOrderItems_Delete @OrderItemId, @RowsAffected OUTPUT", orderItemIdParam, rowsAffectedParam);

            if ((int)rowsAffectedParam.Value > 0)
            {
                // Recalculate total amount
                decimal total = await CalculateOrderTotalAsync(orderId);
                var orderToUpdate = await _context.Orders.FindAsync(orderId);
                if (orderToUpdate != null)
                {
                    orderToUpdate.TotalAmount = total;
                    await UpsertOrderAsync(orderToUpdate);
                }
                return true;
            }
            return false;
        }

        // Calculate total price from order items for a given order
        public async Task<decimal> CalculateOrderTotalAsync(int orderId)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .SumAsync(oi => oi.Quantity * oi.Price);
        }

        // Get selling price of a product
        public async Task<decimal> GetProductSellingPriceAsync(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            return product?.SellingPrice ?? 0;
        }
    }
}
