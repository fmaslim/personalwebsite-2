using PersonalWebsite.Api.DTOs.Common;

namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Orders
{
    public class OrderSearchRequestDto : PagingRequestDto
    {
        public string? Search { get; set; }

        public string? Status { get; set; }

        public decimal? MinTotal { get; set; }

        public decimal? MaxTotal { get; set; }

        public string? SortBy { get; set; }

        public string? SortDir { get; set; }
    }
}
