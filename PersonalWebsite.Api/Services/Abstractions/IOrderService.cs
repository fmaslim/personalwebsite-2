using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.Orders;
using PT = PersonalWebsite.Api.DTOs.PerformanceTraining;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IOrderService
    {
        Task<ServiceResult<int>> CreateOrderAsync(CreateOrderDto dto);
        Task<OrderDetailsDto?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderDetailsDto>> SearchOrdersAsync(int? customerId, byte? status, DateTime? orderDateFrom, DateTime? orderDateTo, int? page, int? pageSize, string? sortBy, string? sortDir);

        // Performance Training
        Task<PagedResponse<OrderSearchResultDto>> SearchOrdersBadN1QueryAsync(PT.OrderSearchRequestDto requestDto);
    }
}
