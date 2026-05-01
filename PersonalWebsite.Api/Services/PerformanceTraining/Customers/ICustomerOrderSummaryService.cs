using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Customers;

namespace PersonalWebsite.Api.Services.PerformanceTraining.Customers
{
    public interface ICustomerOrderSummaryService
    {
        Task<PagedResponse<CustomerOrderSummaryResultDto>> SearchCustomerOrderSummaryAsync(CustomerOrderSummaryRequestDto requestDto);
    }
}
