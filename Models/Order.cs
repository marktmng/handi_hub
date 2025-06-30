using System.Text.Json.Serialization;

namespace DotnetAPI.Models
{
    public class Order
    {
        public int? OrderId { get; set; } // nullable for insert
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now; // default if not set
    }
}
