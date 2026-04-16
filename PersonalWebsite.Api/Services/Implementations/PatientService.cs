using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly AdventureWorksContext _context;
        public PatientService(AdventureWorksContext context)
        {
            _context = context;
        }
        public async Task<PagedPatientSearchResponseDto> SearchPatientsAsync(string? firstName, string? lastName, string? sortBy, string? sortDir, int pageNumber, int pageSize)
        {
            var query = _context.People.AsNoTracking();

            // filter by firstName, check if null
            if ((!string.IsNullOrEmpty(firstName)))
            {
                query = query.Where(p => p.FirstName.Contains(firstName));
            }
            // filter by lastName, check if null
            if ((!string.IsNullOrEmpty(lastName)))
            {
                query = query.Where(p => p.LastName.Contains(lastName));
            }
            // check if SortDir is null, default to ascending
            sortDir = string.IsNullOrEmpty(sortDir) ? "asc" : sortDir;
            if ((!string.IsNullOrEmpty(sortBy)))
                {
                if (sortBy.Equals("firstName", StringComparison.OrdinalIgnoreCase))
                {
                    query = sortDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? query.OrderByDescending(p => p.FirstName) : query.OrderBy(p => p.FirstName);
                }
                else if (sortBy.Equals("lastName", StringComparison.OrdinalIgnoreCase))
                {
                    query = sortDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? query.OrderByDescending(p => p.LastName) : query.OrderBy(p => p.LastName);
                }
                else
                {
                    // default sorting by Id
                    query = sortDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? query.OrderByDescending(p => p.BusinessEntityId) : query.OrderBy(p => p.BusinessEntityId);
                }
            }
            else
            {
                // default sorting by Id
                query = sortDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? query.OrderByDescending(p => p.BusinessEntityId) : query.OrderBy(p => p.BusinessEntityId);
            }
            // get TotalCount after filtering but before paging
            var totalCount = await query.CountAsync();
            // paging
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            // get items after filtering, sorting and paging
            var items = await query.Select(p => new PatientSearchResultDto
            {
                Id = p.BusinessEntityId,
                FirstName = p.FirstName,
                LastName = p.LastName                
            }).ToListAsync();

            return new PagedPatientSearchResponseDto
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = items
            };

            //var result = query.Select(p => new PatientSearchResultDto
            //{
            //    Id = p.BusinessEntityId,
            //    FirstName = p.FirstName,
            //    LastName = p.LastName                
            //}).ToListAsync();

            //return await result;
        }
    }
}
