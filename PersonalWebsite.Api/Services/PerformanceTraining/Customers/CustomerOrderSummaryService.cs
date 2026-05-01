using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Customers;
using PersonalWebsite.Api.Extensions;
using PersonalWebsite.Api.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PersonalWebsite.Api.Services.PerformanceTraining.Customers
{
    public class CustomerOrderSummaryService : ICustomerOrderSummaryService
    {
        private readonly AdventureWorksContext _context;
        public CustomerOrderSummaryService(AdventureWorksContext context)
        {
            _context = context;
        }
        public async Task<PagedResponse<CustomerOrderSummaryResultDto>> SearchCustomerOrderSummaryAsync(CustomerOrderSummaryRequestDto requestDto)
        {
            requestDto.Normalize();
            var query = BuildQuery(_context, requestDto);
            query = ApplySorting(query, requestDto);

            var totalCount = await query.CountAsync();
            query = ApplyPaging(query, requestDto);
            

            var data = await query.ToListAsync();

            return new PagedResponse<CustomerOrderSummaryResultDto>
            {
                Data = data,
                PageNumber = requestDto.PageNumber,
                PageSize = requestDto.PageSize,
                TotalRecords = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)requestDto.PageSize)
            };
        }

        private IQueryable<CustomerOrderSummaryResultDto> ApplySorting(IQueryable<CustomerOrderSummaryResultDto> query, CustomerOrderSummaryRequestDto requestDto)
        {
            query = requestDto.SortBy?.ToLower() switch
            {
                "ordercount" => requestDto.SortDirection?.ToLower() == "asc"
                    ? query.OrderBy(x => x.OrderCount)
                    : query.OrderByDescending(x => x.OrderCount),
                "lastorderdate" => requestDto.SortDirection?.ToLower() == "asc"
                    ? query.OrderBy(x => x.LastOrderDate)
                    : query.OrderByDescending(x => x.LastOrderDate),
                _ => requestDto.SortDirection?.ToLower() == "asc"
                    ? query.OrderBy(x => x.TotalSpent)
                    : query.OrderByDescending(x => x.TotalSpent)
            };

            return query;
        }

        private IQueryable<CustomerOrderSummaryResultDto> ApplyPaging(IQueryable<CustomerOrderSummaryResultDto> query, CustomerOrderSummaryRequestDto requestDto)
        {
            return query
                .Skip((requestDto.PageNumber - 1) * requestDto.PageSize)
                .Take(requestDto.PageSize);
        }

        private IQueryable<CustomerOrderSummaryResultDto> BuildQuery(AdventureWorksContext _context, CustomerOrderSummaryRequestDto requestDto)
        {
            var query = _context.SalesOrderHeaders.AsNoTracking()
                .GroupBy(o => new
                {
                    o.CustomerId,
                    CustomerName = o.Customer.Person != null
                    ? o.Customer.Person.FirstName + " " + o.Customer.Person.LastName
                    : o.Customer.Store != null
                        ? o.Customer.Store.Name
                        : null
                })
                .Select(g => new CustomerOrderSummaryResultDto
                {
                    CustomerId = g.Key.CustomerId,
                    CustomerName = g.Key.CustomerName,
                    OrderCount = g.Count(),
                    TotalSpent = g.Sum(x => x.TotalDue),
                    LastOrderDate = g.Max(x => x.OrderDate)
                });

            if (requestDto.MinOrderCount.HasValue)
            {
                query = query.Where(x => x.OrderCount >= requestDto.MinOrderCount.Value);
            }
            if (requestDto.MinTotalSpent.HasValue)
            {
                query = query.Where(x => x.TotalSpent >= requestDto.MinTotalSpent.Value);
            }

            return query;
        }
    }
}
