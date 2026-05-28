using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.Orders;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Orders;
using PT = PersonalWebsite.Api.DTOs.PerformanceTraining;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IOrderService
    {
        // Task<ServiceResult<int>> CreateOrderAsync(CreateOrderDto dto);
        Task<ServiceResult<DTOs.Orders.CreateOrderResponseDto>> CreateOrderAsync(CreateOrderDto dto);
        // Task<OrderDetailsDto?> GetOrderByIdAsync(int orderId);
        Task<ServiceResult<OrderDetailsDto>> GetOrderByIdAsync(int id);
        Task<ServiceResult<PagedResponse<OrderDetailsDto>>> SearchOrdersAsync(int? customerId, byte? status, DateTime? orderDateFrom, DateTime? orderDateTo, int? page, int? pageSize, string? sortBy, string? sortDir);

        // Performance Training
        Task<PagedResponse<PT.Orders.OrderSearchResultDto>> SearchOrdersBadN1QueryAsync(PT.OrderSearchRequestDto requestDto);
        Task<PagedResponse<PT.Orders.OrderSearchResultDto>> SearchOrdersGoodQueryAsync(PT.OrderSearchRequestDto requestDto);

        Task<ServiceResult<DTOs.Orders.CreateOrderResponseDto>> CreateOrderV2Async(DTOs.Orders.CreateOrderRequestDto dto);
        Task<ServiceResult<DTOs.Orders.CreateOrderResponseDto>> UpdateOrderAsync(int id, DTOs.Orders.UpdateOrderRequestDto dto);
    }
}
