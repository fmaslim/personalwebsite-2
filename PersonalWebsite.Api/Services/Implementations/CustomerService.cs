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

        public async Task<CustomerDetailsDto?> GetCustomerByIdAsync(int customerId)
        {
            if (customerId <= 0)
            {
                return null;
            }

            var customer = await _context.Customers
                .AsNoTracking()
                .Where(c => c.CustomerId == customerId)
                .Select(c => new CustomerDetailsDto
                {
                    CustomerId = c.CustomerId.ToString(),
                    CompanyName = c.Store != null ? c.Store.Name : string.Empty,
                    ContactName = c.Person == null
                        ? string.Empty
                        : ((c.Person.FirstName ?? "") + " " + (c.Person.LastName ?? "")).Trim(),
                    City = string.Empty,
                    Country = string.Empty
                })
                .FirstOrDefaultAsync();

            return customer;
        }
    }
}
