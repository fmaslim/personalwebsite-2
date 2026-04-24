namespace PersonalWebsite.Api.DTOs.Products
{
    public class CreateProductRequestV2Dto
    {
        public string Name { get; set; } = string.Empty;
        public string ProductNumber { get; set; } = string.Empty;
        public decimal? ListPrice { get; set; }
    }
}
