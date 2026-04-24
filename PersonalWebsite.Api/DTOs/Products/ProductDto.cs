namespace PersonalWebsite.Api.DTOs.Products
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal ListPrice { get; set; }
    }
}
