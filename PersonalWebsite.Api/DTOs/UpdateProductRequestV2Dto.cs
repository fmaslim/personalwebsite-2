namespace PersonalWebsite.Api.DTOs
{
    public class UpdateProductRequestV2Dto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ProductNumber { get; set; } = string.Empty;
        public decimal? ListPrice { get; set; }
    }
}
