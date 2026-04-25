using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.Orders;
using PersonalWebsite.Api.DTOs.PerformanceTraining;
using PersonalWebsite.Api.Models;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IOrderServiceV2
    {
        string GetVersionMessage();
        Task<ServiceResult<CreateOrderResponseV2Dto>> CreateOrderAsync(CreateOrderRequestV2Dto dto);

        Task<ServiceResult<CreateOrderResponseV2Dto>> CreateOrderMultiErrorAsync(CreateOrderRequestV2Dto dto);

        Task<ServiceResult<CreateOrderResponseV3Dto>> CreateOrderV3Async(CreateOrderRequestV3Dto dto);
        Task<ServiceResult<UpdateOrderStatusResponseDto>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequestDto dto);
        Task<ServiceResult<GetOrderByIdResponseDto>> GetOrderByIdAsync(int orderId);
        Task<ServiceResult<PagedOrderSummaryResponseDto>> GetAllOrdersAsync(OrderQueryParamsDto query);

        // Performance Training
        Task<ServiceResult<PagedResultDto<OrderSearchResponseDto>>> SearchOrdersAsync(OrderSearchRequestDto request);
    }
}
