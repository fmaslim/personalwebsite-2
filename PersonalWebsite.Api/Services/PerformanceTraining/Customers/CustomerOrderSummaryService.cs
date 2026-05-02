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
        public async Task<ServiceResult<PagedResponse<CustomerOrderSummaryResultDto>>> SearchCustomerOrderSummaryAsync(CustomerOrderSummaryRequestDto requestDto)
        {
            // throw new NullReferenceException("Test null exception");

            // With 2 tools: ValidationResult and Normalize, do both
            var validationResult = requestDto.Validate();
            if (!validationResult.IsValid)
            {
                return ServiceResult<PagedResponse<CustomerOrderSummaryResultDto>>
                    .Fail(validationResult.FieldErrors);
            }

            requestDto.Normalize();
            var query = BuildQuery(_context, requestDto);
            query = query.ApplySorting(requestDto);

            var pagedResponse = await query.ToPagedResponseAsync(
            requestDto.PageNumber,
            requestDto.PageSize);

            if (pagedResponse.TotalRecords == 0)
            {
                return ServiceResult<PagedResponse<CustomerOrderSummaryResultDto>>.NotFound("No customer order summaries were found.");
            }

            return ServiceResult<PagedResponse<CustomerOrderSummaryResultDto>>.Ok(pagedResponse);
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
