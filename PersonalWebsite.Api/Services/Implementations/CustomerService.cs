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
                    CustomerId = c.CustomerId,
                    StoreName = c.Store != null ? c.Store.Name ?? string.Empty : string.Empty,
                    FirstName = c.Person != null ? c.Person.FirstName ?? string.Empty : string.Empty,
                    LastName = c.Person != null ? c.Person.LastName ?? string.Empty : string.Empty,                    
                })
                .FirstOrDefaultAsync();

            return customer;
        }

        public async Task<IEnumerable<CustomerDetailsDto>> SearchCustomersAsync(string? name,
            string? accountNumber,
            int? territoryId,
            string? customerType,
            int page,
            int pageSize,
            string? sortBy,
            string? sortDir)
        {
            var query = _context.Customers.AsNoTracking();

            // normalize param
            customerType = string.IsNullOrWhiteSpace(customerType) ? "all" : customerType.Trim().ToLower();

            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 50 ? 50 : pageSize;

            name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
            accountNumber = string.IsNullOrWhiteSpace(accountNumber) ? null : accountNumber.Trim();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c =>
                    (c.Person != null && (c.Person.FirstName.Contains(name) || c.Person.LastName.Contains(name))) ||
                    (c.Store != null && c.Store.Name.Contains(name)));
            }

            if (!string.IsNullOrEmpty(accountNumber)) {
                query = query.Where(c => c.AccountNumber.Contains(accountNumber));
            }

            if (territoryId.HasValue) {
                query = query.Where(c => c.TerritoryId == territoryId.Value);
            }

            if (customerType == "store")
            {
                query = query.Where(c => c.Store != null);
            }
            else if (customerType == "person")
            {
                query = query.Where(c => c.Person != null);
            }

            sortBy = sortBy?.Trim().ToLower();
            sortDir = sortDir?.Trim().ToLower();

            bool desc = sortDir == "desc";

            if (sortBy == "accountnumber")
            {
                query = desc ? query.OrderByDescending(c => c.AccountNumber) : query.OrderBy(c => c.AccountNumber);
            }
            else if (sortBy == "territoryId")
            {
                query = desc ? query.OrderByDescending(c => c.TerritoryId) : query.OrderBy(c => c.TerritoryId);
            }            
            else if (sortBy == "lastname")
            {
                query = desc
                    ? query.OrderByDescending(c => c.Person != null ? c.Person.LastName : string.Empty)
                    : query.OrderBy(c => c.Person != null ? c.Person.LastName : string.Empty);
            }
            else
            {
                query = desc
                    ? query.OrderByDescending(c => c.CustomerId)
                    : query.OrderBy(c => c.CustomerId);
            }

            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            var customers = query.Select(c => new CustomerDetailsDto
            {
                CustomerId = c.CustomerId,
                StoreName = c.Store != null ? c.Store.Name ?? string.Empty : string.Empty,
                FirstName = c.Person != null ? c.Person.FirstName ?? string.Empty : string.Empty,
                LastName = c.Person != null ? c.Person.LastName ?? string.Empty : string.Empty,
                TerritoryId = c.TerritoryId
            })
            .ToListAsync();

            return await customers;
        }
    }
}
