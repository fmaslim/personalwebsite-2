using Microsoft.EntityFrameworkCore;
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
        //public Task<PagedResponse<OrderSearchResultDto>> SearchOrdersAsync(OrderSearchRequestDto dto)
        //{
        //    var orders = new List<OrderSearchResultDto>();

        //    var orderA = new OrderSearchResultDto();
        //    orderA.OrderId = 1001;
        //    orderA.CustomerName = "John Smith";
        //    orderA.Status = "Pending";
        //    orderA.Total = 149.99m;
        //    orderA.OrderDate = DateTime.UtcNow.AddDays(-1);
        //    orders.Add(orderA);

        //    var orderB = new OrderSearchResultDto();
        //    orderB.OrderId = 1002;
        //    orderB.CustomerName = "Maria Garcia";
        //    orderB.Status = "Shipped";
        //    orderB.Total = 899.50m;
        //    orderB.OrderDate = DateTime.UtcNow.AddDays(-3);
        //    orders.Add(orderB);

        //    var orderC = new OrderSearchResultDto();
        //    orderC.OrderId = 1003;
        //    orderC.CustomerName = "David Lee";
        //    orderC.Status = "Cancelled";
        //    orderC.Total = 59.99m;
        //    orderC.OrderDate = DateTime.UtcNow.AddDays(-5);
        //    orders.Add(orderC);

        //    // Added Search filter
        //    if (!String.IsNullOrEmpty(dto.Search))
        //    {
        //        orders = orders.Where(x => x.CustomerName != null 
        //                                                                                    && x.CustomerName.Contains(dto.Search, StringComparison.OrdinalIgnoreCase)).ToList();
        //    }
        //    //Added status filter
        //    if(!String.IsNullOrEmpty(dto.Status))
        //    {
        //        orders = orders.Where(x => x.Status != null
        //                                                                                    && x.Status.Equals(dto.Status, StringComparison.OrdinalIgnoreCase)).ToList();
        //    }
        //    // Added MinTotal filter
        //    if (dto.MinTotal.HasValue)
        //    {
        //        orders = orders.Where(x => x.Total >= dto.MinTotal.Value).ToList();
        //    }
        //    // Added MaxTotal filter
        //    if (dto.MaxTotal.HasValue)
        //    {
        //        orders = orders.Where(x => x.Total <= dto.MaxTotal.Value).ToList();
        //    }

        //    var a = new OrderSearchResultDto();

        //    // Added sorting
        //    var sortBy = dto.SortBy?.ToLower() ?? dto.SortBy;
        //    var sortDir = dto.SortDir?.ToLower() ?? dto.SortDir;

        //    orders = sortBy switch
        //    {
        //        "total" => sortDir == "asc"
        //            ? orders.OrderBy(x => x.Total).ToList()
        //            : orders.OrderByDescending(x => x.Total).ToList(),
        //        "customer" or "customername" => sortDir == "asc"
        //            ? orders.OrderBy(x => x.CustomerName).ToList()
        //            : orders.OrderByDescending(x => x.CustomerName).ToList(),
        //        "status" => sortDir == "asc" 
        //            ? orders.OrderBy(x => x.Status).ToList()
        //            : orders.OrderByDescending(x => x.Status).ToList(),
        //        "date" or "orderdate" => sortDir == "asc" 
        //            ? orders.OrderBy(x => x.OrderDate).ToList()
        //            : orders.OrderByDescending(x => x.OrderDate).ToList(),
        //        _ => sortDir == "asc" 
        //            ? orders.OrderBy(x => x.OrderDate).ToList()
        //            : orders.OrderByDescending(x => x.OrderDate).ToList()
        //    };

        //    var finalResult = orders.ToPagedResponse(
        //        dto.PageNumber,
        //        dto.PageSize);

        //    return Task.FromResult(finalResult);
        //}

        public async Task<PagedResponse<SearchOrderResultDto>> SearchOrdersAsync(SearchOrderRequestDto dto)
        {
            dto.PageNumber = dto.PageNumber < 1 ? 1 : dto.PageNumber;
            dto.PageSize = dto.PageSize < 1 ? 10 : dto.PageSize;
            dto.PageSize = dto.PageSize > 100 ? 100 : dto.PageSize;
            var query = _context.Orders.AsNoTracking().AsQueryable();

            // Add filters
            if (dto.CustomerId.HasValue)
            {
                query = query.Where(o => o.UserId == dto.CustomerId.Value);
            }
            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                if (Enum.TryParse<OrderStatus>(dto.Status, true, out var status))
                {
                    query = query.Where(o => o.Status == status);
                }
            }
            if (dto.FromDate.HasValue)
            {
                query = query.Where(o => o.CreatedAtUtc >= dto.FromDate.Value);
            }
            if (dto.ToDate.HasValue)
            {
                query = query.Where(o => o.CreatedAtUtc <= dto.ToDate.Value);
            }
            if (dto.MinTotal.HasValue)
            {
                query = query.Where(o => o.TotalAmount >= dto.MinTotal.Value);
            }
            if (dto.MaxTotal.HasValue)
            {
                query = query.Where(o => o.TotalAmount <= dto.MaxTotal.Value);
            }

            // Add sorting
            var sortBy = dto.SortBy?.ToLower();
            var sortDir = dto.SortDir?.ToLower();

            query = sortBy switch
            {
                "totalamount" => sortDir == "asc" ? query.OrderBy(o => o.TotalAmount) : query.OrderByDescending(o => o.TotalAmount),
                "status" => sortDir == "asc" ? query.OrderBy(o => o.Status) : query.OrderByDescending(o => o.Status),
                "createdatutc" => sortDir == "asc" ? query.OrderBy(o => o.CreatedAtUtc) : query.OrderByDescending(o => o.CreatedAtUtc),
                _ => sortDir == "asc" ? query.OrderBy(o => o.CreatedAtUtc) : query.OrderByDescending(o => o.CreatedAtUtc)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((dto.PageNumber - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(o => new SearchOrderResultDto
                {
                    OrderId = o.Id,
                    CustomerId = o.UserId,
                    CustomerName = "", // fill later if you join/include user
                    Status = o.Status.ToString(),
                    OrderDate = o.CreatedAtUtc,
                    TotalAmount = o.TotalAmount
                }).ToListAsync();

            // return items.ToPagedResponse(dto.PageNumber, dto.PageSize);
            return new PagedResponse<SearchOrderResultDto>
            {
                Data = items,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize,
                TotalRecords = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)dto.PageSize),
            };
        }
    }
}
