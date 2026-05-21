using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Models.Errors;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Services.Implementations;

public class PersonV2Service : IPersonV2Service
{
    private readonly AdventureWorksContext _context;
    private readonly ILogger<PersonV2Service> _logger;

    private static readonly string[] _allowedSortBy = { "firstname", "lastname" };
    private static readonly string[] _allowedSortDir = { "asc", "desc" };

    public PersonV2Service(AdventureWorksContext context, ILogger<PersonV2Service> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResult<PagedResponseDto<PersonSearchDto>>> SearchPersonsAsync(
        string name,
        string sortBy,
        string sortDir,
        int pageNumber,
        int pageSize)
    {
        var errors = new List<ServiceError>();

        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(new ServiceError
            {
                Code = "Validation",
                Message = "Name is required.",
                Type = ServiceErrorType.Validation
            });
        }

        var effectiveSortBy = sortBy?.Trim().ToLowerInvariant() ?? string.Empty;
        var effectiveSortDir = sortDir?.Trim().ToLowerInvariant() ?? string.Empty;

        if (!_allowedSortBy.Contains(effectiveSortBy))
        {
            errors.Add(new ServiceError
            {
                Code = "Validation",
                Message = "sortBy must be one of the following values: FirstName, LastName.",
                Type = ServiceErrorType.Validation
            });
        }

        if (!_allowedSortDir.Contains(effectiveSortDir))
        {
            errors.Add(new ServiceError
            {
                Code = "Validation",
                Message = "sortDir must be either Asc or Desc.",
                Type = ServiceErrorType.Validation
            });
        }

        if (pageNumber <= 0 || pageNumber >= 100)
        {
            errors.Add(new ServiceError
            {
                Code = "Validation",
                Message = "pageNumber must be greater than 0 and less than 100.",
                Type = ServiceErrorType.Validation
            });
        }

        if (pageSize <= 0 || pageSize >= 100)
        {
            errors.Add(new ServiceError
            {
                Code = "Validation",
                Message = "pageSize must be greater than 0 and less than 100.",
                Type = ServiceErrorType.Validation
            });
        }

        if (errors.Count > 0)
        {
            var failure = ServiceResult<PagedResponseDto<PersonSearchDto>>.Fail(errors, 400);
            failure.ServiceErrorType = ServiceErrorType.Validation;
            return failure;
        }

        var query = _context.People
            .Where(p => p.FirstName.Contains(name) || p.LastName.Contains(name));

        query = (effectiveSortBy, effectiveSortDir) switch
        {
            ("firstname", "asc")  => query.OrderBy(p => p.FirstName),
            ("firstname", "desc") => query.OrderByDescending(p => p.FirstName),
            ("lastname", "asc")   => query.OrderBy(p => p.LastName),
            ("lastname", "desc")  => query.OrderByDescending(p => p.LastName),
            _ => query.OrderBy(p => p.FirstName)
        };

        var totalCount = await query.CountAsync();
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PersonSearchDto
            {
                BusinessEntityId = p.BusinessEntityId,
                FirstName = p.FirstName,
                LastName = p.LastName
            })
            .ToListAsync();

        var paged = new PagedResponseDto<PersonSearchDto>
        {
            Data = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };

        return ServiceResult<PagedResponseDto<PersonSearchDto>>.Ok(paged);
    }
}
