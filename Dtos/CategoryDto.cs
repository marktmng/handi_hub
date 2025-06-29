namespace DotnetAPI.Dtos
{
    public class CategoryDto
    {
        public string CategoryName { get; set; }

        public CategoryDto() //  Constructor to initialize properties
        {
            // if any property is null, set it to an empty string

            CategoryName = CategoryName ?? string.Empty;
        }
    }
}