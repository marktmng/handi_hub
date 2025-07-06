using DotnetAPI.Models;
using DotnetAPI.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetAPI.Data
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync();
        Task<Payment> UpsertPaymentAsync(Payment payment);
        Task<IEnumerable<PaymentDto>> GetPaymentsByOrderIdAsync(int orderId);
    }
}