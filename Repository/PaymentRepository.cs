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
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DataContext _context;

        public PaymentRepository(DataContext context)
        {
            _context = context;
        }

        // Fetch all available payment methods
        public async Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync()
        {
            return await _context.PaymentMethods
                .FromSqlRaw("EXEC HandiHub.spPaymentMethods_Get")
                .ToListAsync();
        }

        // Insert a new payment for an order
        public async Task<Payment> UpsertPaymentAsync(Payment payment)
        {
            var paymentIdParam = new SqlParameter("@PaymentId", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            var parameters = new[]
            {
                new SqlParameter("@OrderId", payment.OrderId),
                new SqlParameter("@PaymentMethodId", payment.PaymentMethodId),
                new SqlParameter("@PaidAmount", payment.PaidAmount),
                paymentIdParam
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC HandiHub.spPayments_Upsert @OrderId, @PaymentMethodId, @PaidAmount, @PaymentId OUTPUT",
                parameters
            );

            payment.PaymentId = (int?)paymentIdParam.Value;
            return payment;
        }

        // Retrieve payments for a specific order
        public async Task<IEnumerable<PaymentDto>> GetPaymentsByOrderIdAsync(int orderId)
        {
            var orderIdParam = new SqlParameter("@OrderId", orderId);

            return await _context.PaymentDtos
                .FromSqlRaw("EXEC HandiHub.spPayments_GetByOrderId @OrderId", orderIdParam)
                .ToListAsync();
        }
    }
}
