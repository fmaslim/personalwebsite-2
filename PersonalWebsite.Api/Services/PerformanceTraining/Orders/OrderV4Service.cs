using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.Controllers.PerformanceTraining;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Models.Errors;

namespace PersonalWebsite.Api.Services.PerformanceTraining.Orders
{
    public class OrderV4Service : IOrderv4Service
    {
        private readonly AdventureWorksContext _context;
        private readonly ILogger<OrderV4Service> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderV4Service(AdventureWorksContext context, ILogger<OrderV4Service> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResult<string>> CancelOrderAsync(int orderId)
        {
            // Wednesday, 05/06/2026 - Get correlationId
            var correlationId = _httpContextAccessor.HttpContext?
            .Items["CorrelationId"]?
            .ToString();

            var fieldErrors = new List<FieldError>();
            if (orderId <= 0)
            {
                fieldErrors.Add(new FieldError 
                { 
                    Field = "orderId",
                    Message = "Order Id must be greater than 0."
                });
                _logger.LogWarning(
                "Cancel order rejected. CorrelationId={CorrelationId}. OrderId={OrderId}. Reason={Reason}",
                correlationId,
                orderId,
                "Order Id must be greater than 0.");
            }
            if (fieldErrors.Any())
            {
                return ServiceResult<string>.ValidationFail(fieldErrors);
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                _logger.LogWarning(
                "Cancel order rejected. CorrelationId={CorrelationId}. OrderId={OrderId}. Reason={Reason}",
                correlationId,
                orderId,
                "Order was not found");
                return ServiceResult<string>.NotFound("Order was not found.", "orderId");
            }
            if (order.Status == OrderStatus.Shipped) // 3
            {
                _logger.LogWarning(
                "Cancel order rejected. CorrelationId={CorrelationId}. OrderId={OrderId}. Reason={Reason}",
                correlationId,
                orderId,
                "Order was already shipped");
                return ServiceResult<string>.Conflict("Order cannot be cancelled because it has already shipped");
            }
            if (order.Status == OrderStatus.Cancelled) // 4
            {
                _logger.LogWarning(
                "Cancel order rejected. CorrelationId={CorrelationId}. OrderId={OrderId}. Reason={Reason}",
                correlationId,
                orderId,
                "Order is already cancelled");
                return ServiceResult<string>.Conflict("Order is already cancelled.");
            }
            if (order.Status == OrderStatus.Delivered) // 5
            {
                _logger.LogWarning(
                "Cancel order rejected. CorrelationId={CorrelationId}. OrderId={OrderId}. Reason={Reason}",
                correlationId,
                orderId,
                "Unable to cancel. Order has been delivered.");
                return ServiceResult<string>.Conflict("Unable to cancel. Order has been delivered");
            }

            _logger.LogWarning(
                "Cancel order rejected. CorrelationId={CorrelationId}. OrderId={OrderId}. Reason={Reason}",
                correlationId,
                orderId,
                "Order was cancelled successfully");
            return ServiceResult<string>.Ok("Order cancelled successfully");
        }
    }
}
