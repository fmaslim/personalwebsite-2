namespace PersonalWebsite.Api.DTOs.Orders
{
    public class PagedOrderSummaryResponseDto
    {
        public List<OrderSummaryResponseDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
