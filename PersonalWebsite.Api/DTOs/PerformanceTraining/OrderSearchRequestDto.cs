namespace PersonalWebsite.Api.DTOs.PerformanceTraining
{
    public class OrderSearchRequestDto
    {
        public int? CustomerId { get; set; }
        public int? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public string? SortBy { get; set; } = "createdatutc";
        public string? SortDirection { get; set; } = "desc";
    }
}
