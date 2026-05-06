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
        public OrderV4Service(AdventureWorksContext context, ILogger<OrderV4Service> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> CancelOrderAsync(int orderId)
        {
            var fieldErrors = new List<FieldError>();
            if (orderId <= 0)
            {
                fieldErrors.Add(new FieldError 
                { 
                    Field = "orderId",
                    Message = "Order Id must be greater than 0."
                });
                _logger.LogWarning("Cancel order rejected. OrderId={orderId}. Reason={reason}"
                    , orderId,
                    "Order Id must be greater than 0.");
            }
            if (fieldErrors.Any())
            {
                return ServiceResult<string>.ValidationFail(fieldErrors);
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                _logger.LogWarning("Cancel order failed. OrderId={orderId}. Reason={Reason}",
                    orderId,
                    "Order was not found");
                return ServiceResult<string>.NotFound("Order was not found.", "orderId");
            }
            if (order.Status == OrderStatus.Shipped)
            {
                _logger.LogWarning("Cancel order failed. OrderId={orderId}. Reason={Reason}",
                    orderId,
                    "Order cannot be cancelled because it has already shipped");
                return ServiceResult<string>.Conflict("Order cannot be cancelled because it has already shipped");
            }
            if (order.Status == OrderStatus.Cancelled)
            {
                _logger.LogWarning("Cancel order failed. OrderId={orderid}. Reason={Reason}",
                    orderId,
                    "Order is already cancelled.");
                return ServiceResult<string>.Conflict("Order is already cancelled.");
            }
            if (order.Status == OrderStatus.Delivered)
            {
                _logger.LogWarning("Cancel order failed. OrderId={orderid}. Reason={Reason}", 
                    orderId, 
                    "Unable to cancel. Order has been delivered");
                return ServiceResult<string>.Conflict("Unable to cancel. Order has been delivered");
            }

            _logger.LogInformation("Order cancelled successfully. OrderId={orderid}. Reason={Reason}",
                    orderId,
                    "Order is already cancelled.");
            return ServiceResult<string>.Ok("Order cancelled successfully");
        }
    }
}
