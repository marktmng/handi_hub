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
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _context;

        public OrderRepository(DataContext context)
        {
            _context = context;
        }

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
    }
}