namespace PersonalWebsite.Api.DTOs.Products
{
    public class ProductSearchDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductNumber { get; set; }
        public decimal? ListPrice { get; set; }
    }
}
