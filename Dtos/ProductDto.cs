using Microsoft.EntityFrameworkCore;

[Keyless]
public class ProductDto
{

    public string ProductName { get; set; }
    public int CategoryId { get; set; }
    public string ProductDesc { get; set; }
    public string ProductImage { get; set; }
    public decimal ActualPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int Quantity { get; set; }
    public int ArtistId { get; set; }
}


