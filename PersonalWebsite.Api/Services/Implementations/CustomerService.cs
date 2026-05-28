using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Customers;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Models.Errors;
using PersonalWebsite.Api.Services.Abstractions;
using PersonalWebsite.Api.Validation;

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

        public async Task<ServiceResult<CustomerDetailsDto>> GetCustomerByIdAsync(int customerId)
        {
            if (customerId <= 0)
            {
                // return null;
                return ServiceResult<CustomerDetailsDto>.Fail(
                    "CustomerId must be greater than 0.",
                    ServiceErrorType.Validation
                    );
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

            if (customer == null)
            {
                return ServiceResult<CustomerDetailsDto>.NotFound(
                    $"Customer with id {customerId} was not found."
                );
            }
            return ServiceResult<CustomerDetailsDto>.Ok(customer);
        }

        public async Task<ServiceResult<PagedResponse<CustomerOrderDto>>> GetCustomerOrdersAsync(
            int customerId,
            int pageNumber,
            int pageSize,
            string? sortBy,
            string? sortDir,
            string? status,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var errors = new List<string>();
            if (customerId <= 0)
            {
                errors.Add("CustomerId must be greater than 0.");
            }
            if (pageNumber <= 0)
            {
                errors.Add("Page number must be greater than 0.");                
            }
            if (pageSize <= 0)
            {
                errors.Add("Page size must be greater than 0.");
            }
            if (pageSize > 10)
            {
                errors.Add("Page size cannot be greater than 10.");
            }
            if(fromDate.HasValue && toDate.HasValue && fromDate.Value > toDate.Value)
            {
                errors.Add("fromDate cannot be greater than toDate.");
            }

            //validate status
            var allowedStatuses = new[] { "pending", "shipped", "cancelled" };
            if (!string.IsNullOrWhiteSpace(status) && !allowedStatuses.Contains(status.Trim().ToLower()))
            {
                errors.Add($"Invalid status value. Allowed values are: {string.Join(", ", allowedStatuses)}.");
            }

            // normalize sorting params
            sortBy = string.IsNullOrWhiteSpace(sortBy) ? "orderdate" : sortBy.Trim().ToLower();
            sortDir = string.IsNullOrWhiteSpace(sortDir) ? "asc" : sortDir.Trim().ToLower();

            // then validate sorting params
            var allowedSortBy = new[] { "orderdate", "totalamount", "status" };
            var allowedSortDir = new[] { "asc", "desc" };

            if (!allowedSortBy.Contains(sortBy))
            {
                errors.Add($"Invalid sortBy value. Allowed values are: {string.Join(", ", allowedSortBy)}.");
            }
            if (!allowedSortDir.Contains(sortDir))
            {
                errors.Add($"Invalid sortDir value. Allowed values are: {string.Join(", ", allowedSortDir)}.");
            }
            if (errors.Count > 0)
            {
                return ServiceResult<PagedResponse<CustomerOrderDto>>.Fail(errors);
            }

            IQueryable<Order> query = _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == customerId);

            if (!string.IsNullOrWhiteSpace(status))
            {
                var parsedStatus = Enum.Parse<OrderStatus>(status, true);
                query = query.Where(o => o.Status == parsedStatus);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.CreatedAtUtc >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                query = query.Where(o => o.CreatedAtUtc <= toDate.Value);
            }

            query = sortBy switch
            {
                "totalamount" => sortDir == "asc" ? query.OrderBy(o => o.TotalAmount) : query.OrderByDescending(o => o.TotalAmount),
                "status" => sortDir == "asc" ? query.OrderBy(o => o.Status) : query.OrderByDescending(o => o.Status),
                _ => sortDir == "asc" ? query.OrderBy(o => o.CreatedAtUtc) : query.OrderByDescending(o => o.CreatedAtUtc),
            };

            var totalCount = await query.CountAsync();
            var orders = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new CustomerOrderDto
            {
                OrderId = o.Id,
                OrderDate = o.CreatedAtUtc,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString()
            }).ToListAsync();

            return ServiceResult<PagedResponse<CustomerOrderDto>>.Ok(new PagedResponse<CustomerOrderDto>
            {
                Items = orders,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }

        public async Task<ServiceResult<PagedResponse<CustomerDetailsDto>>> SearchCustomersAsync(string? name,
            string? accountNumber,
            int? territoryId,
            string? customerType,
            int page,
            int pageSize,
            string? sortBy,
            string? sortDir)
        {
            var serviceErrors = new List<ServiceError>();
            sortBy = string.IsNullOrWhiteSpace(sortBy) ? "name" : sortBy.Trim().ToLower();
            sortDir = string.IsNullOrWhiteSpace(sortDir) ? "asc" : sortDir.Trim().ToLower();

            PaginationValidation.AddPaginationErrors(serviceErrors, page, pageSize);
            
            // Validate Sort fields
            var allowedSortBy = new[] { "name", "accountnumber", "customertype", "territoryid" };
            var allowedSortDir = new[] { "asc", "desc" };
            SortValidation.AddSortErrors(serviceErrors, sortBy, sortDir, allowedSortBy, allowedSortDir);
            
            if(serviceErrors.Any())
            {
                return ServiceResult<PagedResponse<CustomerDetailsDto>>.Fail(serviceErrors);
            }

            IQueryable<Customer> query = _context.Customers.AsNoTracking();

            name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
            accountNumber = string.IsNullOrWhiteSpace(accountNumber) ? null : accountNumber.Trim();
            customerType = string.IsNullOrWhiteSpace(customerType) ? "all" : customerType.Trim().ToLower();
            
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

            var totalCount = await query.CountAsync();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            var customers = await query.Select(c => new CustomerDetailsDto
            {
                CustomerId = c.CustomerId,
                StoreName = c.Store != null ? c.Store.Name ?? string.Empty : string.Empty,
                FirstName = c.Person != null ? c.Person.FirstName ?? string.Empty : string.Empty,
                LastName = c.Person != null ? c.Person.LastName ?? string.Empty : string.Empty,
                TerritoryId = c.TerritoryId
            })
            .ToListAsync();

            return ServiceResult<PagedResponse<CustomerDetailsDto>>.Ok(new PagedResponse<CustomerDetailsDto>
            {
                Items = customers,
                PageNumber = page,
                PageSize = pageSize,
                TotalRecords = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }
    }
}
