namespace PersonalWebsite.Api.DTOs.Orders
{
    public class OrderSearchRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? CustomerName { get; set; }
        public DateTime? OrderDateFrom { get; set; }
        public DateTime? OrderDateTo { get; set; }
        public decimal? MinTotalDue { get; set; }

        public string? SortBy { get; set; } = "orderDate";
        public string? SortDirection { get; set; } = "desc";
    }
}
