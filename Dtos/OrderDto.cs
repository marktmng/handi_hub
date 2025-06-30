using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Dtos
{
    [Keyless]
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty; // from Users table
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }

    }
}
