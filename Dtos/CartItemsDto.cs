using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Dtos
{
    [Keyless]
    public class CartItemDto
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;  // From Products table

        public int Quantity { get; set; }
        public decimal Price { get; set; }  // Unit price

        public decimal TotalPrice => Quantity * Price;  // Calculated property
    }
}
