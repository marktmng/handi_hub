using DotnetAPI.Models;
using DotnetAPI.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetAPI.Data
{
    public interface ICustomerRepository
    {
        // ✅ Get joined artist + user info (used for GET requests)
        Task<IEnumerable<CustomerDto>> GetCustomersAsync(int? customerId = null);

        // ✅ Create or update artist
        Task<Customer> UpsertCustomerAsync(Customer customer);

        // ✅ Delete artist by ID
        Task<bool> DeleteCustomerAsync(int customerId);
    }
}
