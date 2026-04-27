using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Orders;
using PersonalWebsite.Api.Extensions;
using PersonalWebsite.Api.Models;

namespace PersonalWebsite.Api.Services.PerformanceTraining.Orders
{
    public class OrderSearchTrainingService : IOrderSearchTrainingService
    {
        private readonly AdventureWorksContext _context;
        public OrderSearchTrainingService(AdventureWorksContext context)
        {
            _context = context;
        }
        public Task<PagedResponse<OrderSearchResultDto>> SearchOrdersAsync(OrderSearchRequestDto dto)
        {
            var orders = new List<OrderSearchResultDto>();

            var orderA = new OrderSearchResultDto();
            orderA.OrderId = 1001;
            orderA.CustomerName = "John Smith";
            orderA.Status = "Pending";
            orderA.Total = 149.99m;
            orderA.OrderDate = DateTime.UtcNow.AddDays(-1);
            orders.Add(orderA);

            var orderB = new OrderSearchResultDto();
            orderB.OrderId = 1002;
            orderB.CustomerName = "Maria Garcia";
            orderB.Status = "Shipped";
            orderB.Total = 899.50m;
            orderB.OrderDate = DateTime.UtcNow.AddDays(-3);
            orders.Add(orderB);

            var orderC = new OrderSearchResultDto();
            orderC.OrderId = 1003;
            orderC.CustomerName = "David Lee";
            orderC.Status = "Cancelled";
            orderC.Total = 59.99m;
            orderC.OrderDate = DateTime.UtcNow.AddDays(-5);
            orders.Add(orderC);

            var finalResult = orders.ToPagedResponse(
                dto.PageNumber,
                dto.PageSize);

            return Task.FromResult(finalResult);
        }
    }
}
