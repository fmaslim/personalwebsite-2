using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IOrderService
    {
        Task<ServiceResult<int>> CreateOrderAsync(CreateOrderDto dto);
        Task<OrderDetailsDto?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderDetailsDto>> SearchOrdersAsync(int? customerId, byte? status, DateTime? orderDateFrom, DateTime? orderDateTo, int? page, int? pageSize, string? sortBy, string? sortDir);
    }
}
