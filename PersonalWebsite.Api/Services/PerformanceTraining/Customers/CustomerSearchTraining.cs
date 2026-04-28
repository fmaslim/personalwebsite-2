using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Customers;
using PersonalWebsite.Api.Extensions;
using PersonalWebsite.Api.Models;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            // Tuesday, 04/28/2026 - Added a timer to investigate timing around a query
            var stopwatch = Stopwatch.StartNew();

            var query = _context.Customers
                .AsNoTracking()
                .AsQueryable();

            // Intentionally do a bad query
            // var query = await _context.Customers.AsNoTracking().ToListAsync();
            // var loadedRows = query.Count;

            // Filters go here
            if (!String.IsNullOrEmpty(requestDto.Search))
            {
                query = query.Where(c =>
                // (c.Store != null && c.Store.Name.Contains(requestDto.Search)) ||
                (c.Store != null && c.Store.Name.StartsWith(requestDto.Search)) ||
                (c.Person != null &&
                    (
                        //c.Person.FirstName.Contains(requestDto.Search) ||
                        //c.Person.LastName.Contains(requestDto.Search)
                        c.Person.FirstName.StartsWith(requestDto.Search) ||
                        c.Person.LastName.StartsWith(requestDto.Search)
                    )))
                    ;
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
            }
            ;

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

            var response = await dtoQuery.ToPagedResponseAsync(requestDto.PageNumber, requestDto.PageSize);
            //var totalRecords = dtoQuery.Count();

            //var data = dtoQuery
            //    .Skip((requestDto.PageNumber - 1) * requestDto.PageSize)
            //    .Take(requestDto.PageSize)
            //    .ToList();

            //var response = new PagedResponse<CustomerSearchResultDto>
            //{
            //    Data = data,
            //    PageNumber = requestDto.PageNumber,
            //    PageSize = requestDto.PageSize,
            //    TotalRecords = totalRecords,
            //    TotalPages = (int)Math.Ceiling(totalRecords / (double)requestDto.PageSize)
            //};

            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("====================================");
            Console.WriteLine($"CUSTOMER QUERY TOOK: {elapsedMs} ms");
            //Console.WriteLine($"BAD IN-MEMORY LOADED ROWS: {loadedRows}");
            //Console.WriteLine($"BAD IN-MEMORY FILTERED ROWS: {totalRecords}");
            Console.WriteLine("====================================");
            // Console.WriteLine(dtoQuery.ToQueryString());

            return response;

            
        }

        public async Task<PagedResponse<CustomerSearchResultDto>> SearchCustomersBadFullEntityAsync(CustomerSearchRequestDto requestDto)
        {
            var badStopwatch = Stopwatch.StartNew();
            var query = _context.Customers
            .AsNoTracking()
            .AsQueryable();

            var badFullEntityData = await query
                .OrderByDescending(c => c.CustomerId)
                .Skip((requestDto.PageNumber - 1) * requestDto.PageSize)
                .Take(requestDto.PageSize)
                .ToListAsync();

            badStopwatch.Stop();

            Console.WriteLine("====================================");
            Console.WriteLine($"BAD FULL ENTITY QUERY TOOK: {badStopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"BAD FULL ENTITY QUERY ROWS: {badFullEntityData.Count}");
            Console.WriteLine("====================================");

            // return badFullEntityData.ToPagedResponseAsync(requestDto.PageNumber, requestDto.PageSize);
            var data = badFullEntityData.Select(c => new CustomerSearchResultDto
            {
                CustomerId = c.CustomerId,
                CompanyName = c.Store != null ? c.Store.Name : null,
                ContactName = c.Person != null
        ? c.Person.FirstName + " " + c.Person.LastName
        : null,
                City = "City Placeholder",
                Country = "Country Placeholder"
            }).ToList();

            return new PagedResponse<CustomerSearchResultDto>
            {
                Data = data,
                PageNumber = requestDto.PageNumber,
                PageSize = requestDto.PageSize,
                TotalRecords = data.Count,
                TotalPages = (int)Math.Ceiling(data.Count / (double)requestDto.PageSize)
            };
        }

        public async Task<PagedResponse<CustomerSearchResultDto>> SearchCustomersBadN1QueryAsync(CustomerSearchRequestDto requestDto)
        {
            var badStopwatch = Stopwatch.StartNew();
            
            var customers = await _context.Customers.AsNoTracking()
                .OrderByDescending(c => c.CustomerId)
                .Skip((requestDto.PageNumber - 1) * requestDto.PageSize)
                .Take(requestDto.PageSize)
                .ToListAsync();

            // badStopwatch.Stop();

            var data = new List<CustomerSearchResultDto>();
            // ************************************
            // *** Intentionally bad N+1 part ***
            // ************************************
            foreach (var customer in customers)
            {
                var store = customer.StoreId != null
                        ? await _context.Stores
                            .AsNoTracking()
                            .FirstOrDefaultAsync(s => s.BusinessEntityId == customer.StoreId)
                        : null;

                var person = customer.PersonId != null
                    ? await _context.People
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.BusinessEntityId == customer.PersonId)
                    : null;

                data.Add(new CustomerSearchResultDto
                {
                    CustomerId = customer.CustomerId,
                    CompanyName = store != null ? store.Name : null,
                    ContactName = person != null
                ? person.FirstName + " " + person.LastName
                : null,
                    City = "City Placeholder",
                    Country = "Country Placeholder"
                });
            }

            /*
             * for API response DTOs, projection is usually the better move ✅
            Include vs Projection
            Use Include when you need the full related entity

            Use projection when building an API response
             var data = await _context.Customers
            .AsNoTracking()
            .OrderByDescending(c => c.CustomerId)
            .Select(c => new CustomerSearchResultDto // *** here projection ***
            {
                CustomerId = c.CustomerId,
                CompanyName = c.Store != null ? c.Store.Name : null, // *** here ***
                ContactName = c.Person != null // *** here ***
                    ? c.Person.FirstName + " " + c.Person.LastName
                    : null,
                City = "City Placeholder",
                Country = "Country Placeholder"
            })
            .Skip((requestDto.PageNumber - 1) * requestDto.PageSize)
            .Take(requestDto.PageSize)
            .ToListAsync();
             */

            badStopwatch.Stop();

            Console.WriteLine("====================================");
            Console.WriteLine($"BAD N+1 QUERY TOOK: {badStopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"BAD N+1 CUSTOMERS LOADED: {customers.Count}");
            Console.WriteLine("====================================");

            return new PagedResponse<CustomerSearchResultDto>
            {
                Data = data,
                PageNumber = requestDto.PageNumber,
                PageSize = requestDto.PageSize,
                TotalRecords = data.Count,
                TotalPages = (int)Math.Ceiling(data.Count / (double)requestDto.PageSize)
            };
        }

        public async Task<PagedResponse<CustomerSearchResultDto>> SearchCustomersGoodQueryAsync(CustomerSearchRequestDto requestDto)
        {
            var data = _context.Customers
                .AsNoTracking()
                /*
                 * Go to SQL.
                    Join what you need.
                    Return only these DTO fields.
                    Do it in one database trip.
                 */
                .Select(c => new CustomerSearchResultDto // instead of looping through all Customers, it's done through projection
                {
                    CustomerId = c.CustomerId,
                    CompanyName = c.Store != null ? c.Store.Name : null,
                    ContactName = c.Person != null ? c.Person.FirstName + " " + c.Person.LastName : null,
                });

            var totalCount = await data.CountAsync();

            var pagedData = await data
                .OrderBy(c => c.CustomerId)
                .Skip((requestDto.PageNumber - 1) * requestDto.PageSize)
                .Take(requestDto.PageSize)
                .ToListAsync();

            return new PagedResponse<CustomerSearchResultDto>
            {
                Data = pagedData,
                PageNumber = requestDto.PageNumber,
                PageSize = requestDto.PageSize,
                TotalRecords = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)requestDto.PageSize)
            };
                
        }
    }
}
