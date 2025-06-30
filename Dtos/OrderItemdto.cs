using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Dtos
{
    [Keyless]
    public class OrderItemDto
    {
        public int OrderItemId { get; set; }        // Primary key from OrderItems
        public int OrderId { get; set; }            // Link to Orders
        public int ProductId { get; set; }          // Link to Products
        public string ProductName { get; set; } = string.Empty; // Optional JOINed name
        public int Quantity { get; set; }           // Integer quantity
        public decimal Price { get; set; }          // Unit price
    }
}
