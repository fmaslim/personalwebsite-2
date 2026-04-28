using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.PerformanceTraining;
using PT = PersonalWebsite.Api.DTOs.PerformanceTraining.Orders;

namespace PersonalWebsite.Api.Services.PerformanceTraining.Orders
{
    public interface IOrderSearchTrainingService
    {
        // Task<PagedResponse<PT.OrderSearchResultDto>> SearchOrdersAsync(PT.OrderSearchRequestDto dto);

        Task<PagedResponse<PT.SearchOrderResultDto>> SearchOrdersAsync(PT.SearchOrderRequestDto dto);
    }
}
