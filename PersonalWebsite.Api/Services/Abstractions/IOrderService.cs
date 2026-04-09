using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(CreateOrderDto dto);
        Task<OrderDetailsDto?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderDetailsDto>> SearchOrdersAsync(int? customerId, byte? status);
    }
}
