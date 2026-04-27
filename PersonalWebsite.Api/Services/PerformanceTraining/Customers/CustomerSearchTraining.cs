using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Customers;
using PersonalWebsite.Api.Extensions;
using PersonalWebsite.Api.Models;

namespace PersonalWebsite.Api.Services.PerformanceTraining.Customers
{
    public class CustomerSearchTraining : ICustomerSearchTraining
    {
        private readonly AdventureWorksContext _context;
        public CustomerSearchTraining(AdventureWorksContext context)
        {
            _context = context;
        }
        public async Task<PagedResponse<CustomerSearchResultDto>> SearchCustomersAsync(CustomerSearchRequestDto requestDto)
        {
            var query = _context.Customers.AsNoTracking().AsQueryable();

            // Filters go here
            if (!String.IsNullOrEmpty(requestDto.Search))
            {
                query = query.Where(c =>
                (c.Store != null && c.Store.Name.Contains(requestDto.Search)) ||
                (c.Person != null &&
                    (
                        c.Person.FirstName.Contains(requestDto.Search) ||
                        c.Person.LastName.Contains(requestDto.Search)
                    )));
            }
            // Sorting goes here
            var sortBy = requestDto.SortBy?.ToLower();
            var sortDir = requestDto.SortDir?.ToLower();
            query = sortBy switch
            { 
                "company" or "companyname" => sortDir == "asc" ? query.OrderBy(c => c.Store!.Name)
                                                                                                    : query.OrderByDescending(c => c.Store!.Name),
                "contact" or "contactname" => sortDir == "asc" ? query.OrderBy(c => c.Person!.FirstName).ThenBy(c => c.Person!.LastName)
                                                                                               : query.OrderByDescending(c => c.Person!.FirstName).ThenByDescending(c => c.Person!.LastName),
                _ => sortDir == "asc" ? query.OrderBy(c => c.CustomerId)
                                                    : query.OrderByDescending(c => c.CustomerId),
            };

            var dtoQuery = query.Select(c => new CustomerSearchResultDto
            {
                CustomerId = c.CustomerId,
                CompanyName = c.Store != null ? c.Store.Name : null,
                ContactName = c.Person != null
                    ? c.Person.FirstName + " " + c.Person.LastName
                    : null,
                City = "City Placeholder",
                Country = "Country Placeholder"
            });

            return await dtoQuery.ToPagedResponseAsync(requestDto.PageNumber, requestDto.PageSize);
        }
    }
}
