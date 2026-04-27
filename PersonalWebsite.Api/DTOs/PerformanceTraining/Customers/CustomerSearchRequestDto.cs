using PersonalWebsite.Api.DTOs.Common;

namespace PersonalWebsite.Api.DTOs.PerformanceTraining.Customers
{
    public class CustomerSearchRequestDto : PagingRequestDto
    {
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public string? SortDir { get; set; }        
    }
}
