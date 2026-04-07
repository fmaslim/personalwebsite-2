using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly AdventureWorksContext _context;
        public CustomerService(AdventureWorksContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            var query = await _context.Customers
                .AsNoTracking()
                .OrderBy(c => c.CustomerId)
                .Take(10)
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    AccountNumber = c.AccountNumber
                })
                .ToListAsync();

            return query;
        }
    }
}
