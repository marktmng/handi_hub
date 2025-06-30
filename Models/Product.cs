using System.Text.Json.Serialization;

namespace DotnetAPI.Models
{
    public class Product
    {
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; } // Assuming CategoryId is a foreign key to a Category entity
        public string ProductDesc { get; set; }
        public string ProductImage { get; set; }
        public decimal ActualPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int Quantity { get; set; }
        public int ArtistId { get; set; } // Assuming ArtistId is a foreign key to an Artist entity
    }

}
