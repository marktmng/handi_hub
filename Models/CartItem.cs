namespace DotnetAPI.Models
{
    public class CartItem
    {
        public int? CartId { get; set; }  // Nullable for new inserts
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }  // Unit price
    }
}
