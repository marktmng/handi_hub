using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using System.Threading.Tasks;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // ✅ GET (all or by ID) — returns ArtistDto ( user details)
        [HttpGet("GetCustomers")]
        public async Task<IActionResult> GetArtists([FromQuery] int? customerId = null)
        {
            var customers = await _customerRepository.GetCustomersAsync(customerId);
            return Ok(customers);
        }

        // ✅ POST create or update
        [HttpPost("UpsertCustomer")]
        public async Task<IActionResult> UpsertCustomerAsync([FromBody] Customer customer)
        {
            if (customer == null)
                return BadRequest("Customer is null.");

            var result = await _customerRepository.UpsertCustomerAsync(customer);

            if (result != null && result.CustomerId.HasValue)
                return Ok(result); // returns basic Artist model (no user details)

            return BadRequest("Failed to process customer data.");
        }

        // ✅ PUT update existing artist by ID
        [HttpPut("Update/{customerId}")]
        public async Task<IActionResult> UpdateCustomer(int customerId, [FromBody] Customer customer)
        {
            if (customer == null || customer.CustomerId != customerId)
                return BadRequest("Customer ID mismatch or customer is null.");

            var updated = await _customerRepository.UpsertCustomerAsync(customer);

            if (updated != null && updated.CustomerId.HasValue)
                return Ok(updated);

            return BadRequest("Failed to update customer.");
        }

        // ✅ DELETE artist by ID
        [HttpDelete("Delete/{customerId}")]
        public async Task<IActionResult> DeleteCustomer(int customerId)
        {
            var deleted = await _customerRepository.DeleteCustomerAsync(customerId);

            if (deleted)
                return Ok($"Customer with ID {customerId} deleted successfully.");

            return NotFound($"Customer with ID {customerId} was not found.");
        }
    }
}
