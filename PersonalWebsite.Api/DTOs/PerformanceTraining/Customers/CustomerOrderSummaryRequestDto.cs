namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Customers
{
    public class CustomerOrderSummaryRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int? MinOrderCount { get; set; }
        public decimal? MinTotalSpent { get; set; }

        public string? SortBy { get; set; } = "totalSpent";
        public string? SortDirection { get; set; } = "desc";
    }
}
