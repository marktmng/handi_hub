using Microsoft.EntityFrameworkCore;

[Keyless]
public class ProductDto
{

    public string ProductName { get; set; }
    public int CategoryId { get; set; } // Assuming CategoryId is a foreign key to a Category entity
    public string ProductDesc { get; set; }
    public string ProductImage { get; set; }
    public decimal ActualPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int Quantity { get; set; }
    public int ArtistId { get; set; } // Assuming ArtistId is a foreign key to an Artist entity

    public ProductDto() //  Constructor to initialize properties
    {
        // if any property is null, set it to an empty string

        ProductName = ProductName ?? string.Empty;
        ProductDesc = ProductDesc ?? string.Empty;
        ProductImage = ProductImage ?? string.Empty;
        ActualPrice = ActualPrice == 0 ? 0 : ActualPrice; // Assuming 0 is a valid default value
        SellingPrice = SellingPrice == 0 ? 0 : SellingPrice; // Assuming 0 is a valid default value
        Quantity = Quantity == 0 ? 0 : Quantity; // Assuming 0 is a valid default value
        CategoryId = CategoryId == 0 ? 0 : CategoryId; // Assuming 0 is a valid default value
        ArtistId = ArtistId == 0 ? 0 : ArtistId;
    }
}


