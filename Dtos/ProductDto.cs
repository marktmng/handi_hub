using Microsoft.AspNetCore.Http; // for IFormFile

namespace DotnetAPI.Dtos
{
    public class ProductDto
    {
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public string ProductDesc { get; set; }
        public IFormFile ProductImageFile { get; set; }  // <-- For uploaded image
        public string ProductImage { get; set; }          // <-- For existing image URL (optional)
        public decimal ActualPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int Quantity { get; set; }
        public int ArtistId { get; set; }
    }
}
