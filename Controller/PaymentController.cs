using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Models;
using DotnetAPI.Repository;
using DotnetAPI.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        // Get all available payment methods
        // [Authorize]
        [HttpGet("Methods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var methods = await _paymentRepository.GetPaymentMethodsAsync();
            return Ok(methods);
        }

        // Add payment for an order
        // [Authorize]
        [HttpPost("Pay")]
        public async Task<IActionResult> Pay([FromBody] Payment payment)
        {
            if (payment == null)
                return BadRequest("Payment is null.");

            if (payment.PaidAmount <= 0)
                return BadRequest("PaidAmount must be greater than zero.");

            var result = await _paymentRepository.UpsertPaymentAsync(payment);
            return Ok(result);
        }

        // Retrieve payment info by order ID
        // [Authorize]
        [HttpGet("Order/{orderId}")]
        public async Task<IActionResult> GetPaymentsByOrderId(int orderId)
        {
            var payments = await _paymentRepository.GetPaymentsByOrderIdAsync(orderId);
            return Ok(payments);
        }
    }
}