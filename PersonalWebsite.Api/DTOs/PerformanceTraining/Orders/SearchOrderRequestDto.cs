namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Orders
{
    public class SearchOrderRequestDto
    {
        public int? CustomerId { get; set; }

        public string? Status { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public decimal? MinTotal { get; set; }

        public decimal? MaxTotal { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? SortBy { get; set; }

        public string? SortDir { get; set; } = "desc";
    }
}
