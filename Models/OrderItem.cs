namespace DotnetAPI.Models
{
    public class OrderItem
    {
        public int? OrderItemId { get; set; } // Nullable for new inserts
        public int OrderId { get; set; }      // Foreign key
        public int ProductId { get; set; }    // Foreign key
        public int Quantity { get; set; }     // Whole units only
        public decimal Price { get; set; }    // Unit price
    }
}
