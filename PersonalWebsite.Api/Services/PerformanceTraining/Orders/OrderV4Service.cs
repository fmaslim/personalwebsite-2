using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.Models;

namespace PersonalWebsite.Api.Services.PerformanceTraining.Orders
{
    public class OrderV4Service : IOrderv4Service
    {
        private readonly AdventureWorksContext _context;
        public OrderV4Service(AdventureWorksContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<string>> CancelOrderAsync(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return ServiceResult<string>.NotFound("Order was not found.", "orderId");
            }
            if (order.Status == OrderStatus.Shipped)
            {
                return ServiceResult<string>.Conflict("Order cannot be cancelled because it has already shipped");
            }
            if (order.Status == OrderStatus.Cancelled)
            {
                return ServiceResult<string>.Conflict("Order is already cancelled.");
            }
            if (order.Status == OrderStatus.Delivered)
            {
                return ServiceResult<string>.Conflict("Unable to cancel. Order has been delivered");
            }

            return ServiceResult<string>.Ok("Order cancelled successfully");
        }
    }
}
