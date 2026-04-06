namespace PersonalWebsite.Api.DTOs
{
    public class ProductListResponseDto
    {
        public int Count { get; set; }
        public IEnumerable<ProductListDto> Items { get; set; } = new List<ProductListDto>();
    }
}
