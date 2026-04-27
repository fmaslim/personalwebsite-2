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

            // Added Search filter
            if (!String.IsNullOrEmpty(dto.Search))
            {
                orders = orders.Where(x => x.CustomerName != null 
                                                                                            && x.CustomerName.Contains(dto.Search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            //Added status filter
            if(!String.IsNullOrEmpty(dto.Status))
            {
                orders = orders.Where(x => x.Status != null
                                                                                            && x.Status.Equals(dto.Status, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            // Added MinTotal filter
            if (dto.MinTotal.HasValue)
            {
                orders = orders.Where(x => x.Total >= dto.MinTotal.Value).ToList();
            }
            // Added MaxTotal filter
            if (dto.MaxTotal.HasValue)
            {
                orders = orders.Where(x => x.Total <= dto.MaxTotal.Value).ToList();
            }

            var a = new OrderSearchResultDto();

            // Added sorting
            var sortBy = dto.SortBy?.ToLower() ?? dto.SortBy;
            var sortDir = dto.SortDir?.ToLower() ?? dto.SortDir;

            orders = sortBy switch
            {
                "total" => sortDir == "asc"
                    ? orders.OrderBy(x => x.Total).ToList()
                    : orders.OrderByDescending(x => x.Total).ToList(),
                "customer" or "customername" => sortDir == "asc"
                    ? orders.OrderBy(x => x.CustomerName).ToList()
                    : orders.OrderByDescending(x => x.CustomerName).ToList(),
                "status" => sortDir == "asc" 
                    ? orders.OrderBy(x => x.Status).ToList()
                    : orders.OrderByDescending(x => x.Status).ToList(),
                "date" or "orderdate" => sortDir == "asc" 
                    ? orders.OrderBy(x => x.OrderDate).ToList()
                    : orders.OrderByDescending(x => x.OrderDate).ToList(),
                _ => sortDir == "asc" 
                    ? orders.OrderBy(x => x.OrderDate).ToList()
                    : orders.OrderByDescending(x => x.OrderDate).ToList()
            };

            var finalResult = orders.ToPagedResponse(
                dto.PageNumber,
                dto.PageSize);

            return Task.FromResult(finalResult);
        }
    }
}
