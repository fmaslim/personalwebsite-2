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
            // Sorting goes here

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
