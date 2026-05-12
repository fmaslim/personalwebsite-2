using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.PerformanceTraining;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Orders;
using PT = PersonalWebsite.Api.DTOs.PerformanceTraining.Orders;

namespace PersonalWebsite.Api.Services.PerformanceTraining.Orders
{
    public interface IOrderSearchTrainingService
    {
        // Task<PagedResponse<PT.SearchOrderResultDto>> SearchOrdersAsync(PT.SearchOrderRequestDto dto);
        Task<ServiceResult<PagedResponse<SearchOrderResultDto>>> SearchOrdersAsync(PT.SearchOrderRequestDto dto);
    }
}
