using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.Patients;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Models.Errors;
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
        //public async Task<PagedPatientSearchResponseDto> SearchPatientsAsync(string? firstName, string? lastName, string? sortBy, string? sortDir, int pageNumber, int pageSize)
        //{
        //    var query = _context.People.AsNoTracking();

        //    // filter by firstName, check if null
        //    if ((!string.IsNullOrEmpty(firstName)))
        //    {
        //        query = query.Where(p => p.FirstName.Contains(firstName));
        //    }
        //    // filter by lastName, check if null
        //    if ((!string.IsNullOrEmpty(lastName)))
        //    {
        //        query = query.Where(p => p.LastName.Contains(lastName));
        //    }
        //    // check if SortDir is null, default to ascending
        //    sortDir = string.IsNullOrEmpty(sortDir) ? "asc" : sortDir;
        //    if ((!string.IsNullOrEmpty(sortBy)))
        //        {
        //        if (sortBy.Equals("firstName", StringComparison.OrdinalIgnoreCase))
        //        {
        //            query = sortDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? query.OrderByDescending(p => p.FirstName) : query.OrderBy(p => p.FirstName);
        //        }
        //        else if (sortBy.Equals("lastName", StringComparison.OrdinalIgnoreCase))
        //        {
        //            query = sortDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? query.OrderByDescending(p => p.LastName) : query.OrderBy(p => p.LastName);
        //        }
        //        else
        //        {
        //            // default sorting by Id
        //            query = sortDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? query.OrderByDescending(p => p.BusinessEntityId) : query.OrderBy(p => p.BusinessEntityId);
        //        }
        //    }
        //    else
        //    {
        //        // default sorting by Id
        //        query = sortDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? query.OrderByDescending(p => p.BusinessEntityId) : query.OrderBy(p => p.BusinessEntityId);
        //    }
        //    // get TotalCount after filtering but before paging
        //    var totalCount = await query.CountAsync();
        //    // paging
        //    //pageNumber = pageNumber <= 0 ? 1 : pageNumber;            
        //    //pageSize = pageSize <= 0 ? 10 : pageSize;
        //    //pageSize = pageSize > 100 ? 100 : pageSize; // limit page size to 100
        //    query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        //    // get items after filtering, sorting and paging
        //    var items = await query.Select(p => new PatientSearchResultDto
        //    {
        //        Id = p.BusinessEntityId,
        //        FirstName = p.FirstName,
        //        LastName = p.LastName                
        //    }).ToListAsync();

        //    return new PagedPatientSearchResponseDto
        //    {
        //        TotalCount = totalCount,
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        Items = items
        //    };
        //}

        public async Task<ServiceResult<PagedPatientSearchResponseDto>> SearchPatientsAsync(string? firstName, string? lastName, string? sortBy, string? sortDir, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
            {
                return ServiceResult<PagedPatientSearchResponseDto>.Fail(
                    "PageNumber must be greater than 0.", 
                    Models.Errors.ServiceErrorType.Validation);
            }
            if (pageSize <= 0)
            {
                return ServiceResult<PagedPatientSearchResponseDto>.Fail(
                    "PageSize must be greater than 0.",
                    Models.Errors.ServiceErrorType.Validation);
            }
            if (pageSize > 100)
            {
                return ServiceResult<PagedPatientSearchResponseDto>.Fail(
                    "PageSize must be less than or equal to 100.",
                    ServiceErrorType.Validation);
            }
            var query = _context.People.AsNoTracking().AsQueryable();

            // filter by firstName, check if Null
            if (!string.IsNullOrWhiteSpace(firstName))
            {
                query = query.Where(p => p.FirstName.Contains(firstName));
            }
            if (!string.IsNullOrWhiteSpace(lastName))
            {
                query = query.Where(p => p.FirstName.Contains(lastName));
            }
            // sort
            var allowedSortBy = new[] { "id", "firstName", "lastName" };

            if (!string.IsNullOrWhiteSpace(sortBy) &&
                !allowedSortBy.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            {
                return ServiceResult<PagedPatientSearchResponseDto>.Fail(
                    "SortBy is invalid. Allowed values are: id, firstName, lastName.",
                    ServiceErrorType.Validation);
            }
            var allowedSortDir = new[] { "asc", "desc" };

            if (!string.IsNullOrWhiteSpace(sortDir) &&
                !allowedSortDir.Contains(sortDir, StringComparer.OrdinalIgnoreCase))
            {
                return ServiceResult<PagedPatientSearchResponseDto>.Fail(
                    "SortDir is invalid. Allowed values are: asc, desc.",
                    ServiceErrorType.Validation);
            }
            // set default sort value
            sortBy = string.IsNullOrWhiteSpace(sortBy) ? "id" : sortBy.ToLower();
            sortDir = string.IsNullOrWhiteSpace(sortDir) ? "asc" : sortDir.ToLower();
            //sortDir = string.IsNullOrEmpty(sortDir) ? "asc" : sortDir.ToLower();
            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy.Equals("firstName", StringComparison.OrdinalIgnoreCase))
                {
                    query = sortDir == "asc" ? query.OrderBy(p => p.FirstName) : query.OrderByDescending(p => p.FirstName);
                }
                else if (sortBy.Equals("lastName", StringComparison.OrdinalIgnoreCase))
                {
                    query = sortDir == "asc" ? query.OrderBy(p => p.LastName) : query.OrderByDescending(p => p.LastName);
                }
                else
                {
                    // default sorting by id
                    query = sortDir == "asc" ? query.OrderBy(p => p.BusinessEntityId) : query.OrderByDescending(p => p.BusinessEntityId);
                }
            }
            else
            {
                // if sortBy is empty, default sorting by id
                query = sortDir == "asc" ? query.OrderBy(p => p.BusinessEntityId) : query.OrderByDescending(p => p.BusinessEntityId);
            }

            // get TotalCount after filtering
            var totalCount = await query.CountAsync();

            // paging

            // rather than silently setting the number here, it's better to validate them and fail 
            //pageNumber = pageNumber <= 0 ? 0 : pageNumber;
            //pageSize = pageSize <= 0 ? 0 : pageSize;
            //pageSize = pageSize >= 100 ? 100 : pageSize;
            query = query.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // project after filtering and paging
            var items = await query.Select(p => new PatientSearchResultDto
            {
                Id = p.BusinessEntityId,
                FirstName = p.FirstName,
                LastName = p.LastName
            }).ToListAsync();

            var pagedResponse = new PagedPatientSearchResponseDto
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = items
            };

            return ServiceResult<PagedPatientSearchResponseDto>.Ok(pagedResponse);
        }
    }
}
