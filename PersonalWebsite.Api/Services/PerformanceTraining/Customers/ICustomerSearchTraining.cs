using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Customers;

namespace PersonalWebsite.Api.Services.PerformanceTraining.Customers
{
    public interface ICustomerSearchTraining
    {
        Task<PagedResponse<CustomerSearchResultDto>> SearchCustomersAsync(CustomerSearchRequestDto requestDto);

        Task<PagedResponse<CustomerSearchResultDto>> SearchCustomersBadFullEntityAsync(CustomerSearchRequestDto requestDto);

        Task<PagedResponse<CustomerSearchResultDto>> SearchCustomersBadN1QueryAsync(CustomerSearchRequestDto requestDto);

        Task<PagedResponse<CustomerSearchResultDto>>SearchCustomersGoodQueryAsync(CustomerSearchRequestDto requestDto);
    }
}
