using PersonalWebsite.Api.DTOs.Common;

namespace PersonalWebsite.Api.Services.PerformanceTraining.Orders
{
    public interface IOrderv4Service
    {
        Task<ServiceResult<string>> CancelOrderAsync(int orderId);
    }
}
