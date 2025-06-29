using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetAPI.Repository
{
    public class CustomerRepository : ICustomerRepository // Interface for customer repository
    {
        private readonly DataContext _context; // DataContext to access the database

        public CustomerRepository(DataContext context)
        {
            _context = context;
        }

        // ✅ Get artists with optional filter
        public async Task<IEnumerable<CustomerDto>> GetCustomersAsync(int? customerId = null)
        {
            var customerIdParam = new SqlParameter("@CustomerId", customerId.HasValue ? customerId.Value : (object)DBNull.Value);

            return await _context.CustomerDtos
                .FromSqlRaw("EXEC HandiHub.spCustomers_Get @CustomerId", customerIdParam)
                .ToListAsync();
        }

        // ✅ Insert or update customer
        public async Task<Customer> UpsertCustomerAsync(Customer customer)
        {
            var customerIdParam = new SqlParameter("@CustomerId", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.InputOutput,
                Value = customer.CustomerId ?? (object)DBNull.Value
            };

            var userIdParam = new SqlParameter("@UserId", customer.UserId);

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC HandiHub.spCustomers_Upsert @CustomerId OUTPUT, @UserId",
                customerIdParam, userIdParam);

            customer.CustomerId = (int?)customerIdParam.Value;
            return customer;
        }

        // ✅ Delete customer by ID
        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            var customerIdParam = new SqlParameter("@CustomerId", customerId);
            var rowsAffectedParam = new SqlParameter("@RowsAffected", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC HandiHub.spCustomers_Delete @CustomerId, @RowsAffected OUTPUT",
                customerIdParam, rowsAffectedParam);

            return (int)rowsAffectedParam.Value > 0;
        }
    }
}
