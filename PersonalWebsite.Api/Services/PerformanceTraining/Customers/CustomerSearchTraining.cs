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
    }
}
