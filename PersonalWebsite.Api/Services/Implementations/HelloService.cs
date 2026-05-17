using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Models.Errors;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class HelloService : IHelloService
    {
        private readonly ILogger<HelloService> _logger;

        private static readonly List<PersonDto> _persons = new()
        {
            new PersonDto { Id = 1,  Name = "Alice",   Age = 30 },
            new PersonDto { Id = 2,  Name = "Bob",     Age = 25 },
            new PersonDto { Id = 3,  Name = "Charlie", Age = 35 },
            new PersonDto { Id = 4,  Name = "Diana",   Age = 28 },
            new PersonDto { Id = 5,  Name = "Edward",  Age = 42 },
            new PersonDto { Id = 6,  Name = "Fiona",   Age = 31 },
            new PersonDto { Id = 7,  Name = "Franky",  Age = 27 },
            new PersonDto { Id = 8,  Name = "George",  Age = 45 },
            new PersonDto { Id = 9,  Name = "Hannah",  Age = 22 },
            new PersonDto { Id = 10, Name = "Ian",     Age = 38 },
            new PersonDto { Id = 11, Name = "Julia",   Age = 29 },
            new PersonDto { Id = 12, Name = "Kevin",   Age = 33 },
            new PersonDto { Id = 13, Name = "Laura",   Age = 26 },
            new PersonDto { Id = 14, Name = "Mike",    Age = 50 },
            new PersonDto { Id = 15, Name = "Nina",    Age = 24 }
        };

        private static readonly string[] _allowedSortBy = { "id", "name", "age" };
        private static readonly string[] _allowedSortDir = { "asc", "desc" };

        public HelloService(ILogger<HelloService> logger)
        {
            _logger = logger;
        }

        public async Task<ServiceResult<HelloResponseDto>> GetGreetingAsync(string name)
        {
            // ensure this method is asynchronous for API consistency
            await Task.Yield();

            if (string.IsNullOrWhiteSpace(name))
            {
                var fieldErrors = new List<FieldError>
                {
                    new FieldError { Field = "name", Message = "Name is required." }
                };

                return ServiceResult<HelloResponseDto>.ValidationFail(fieldErrors);
            }

            if (name.Length > 50)
            {
                var fieldErrors = new List<FieldError>
                {
                    new FieldError { Field = "name", Message = "Name cannot be longer than 50 characters." }
                };

                return ServiceResult<HelloResponseDto>.ValidationFail(fieldErrors);
            }

            var dto = new HelloResponseDto
            {
                Message = $"Hello, {name}!"
            };

            return ServiceResult<HelloResponseDto>.Ok(dto);
        }

        public async Task<ServiceResult<PagedResponseDto<PersonDto>>> GetPersonsAsync(
            string? name,
            string? sortBy,
            string? sortDir,
            int pageNumber,
            int pageSize)
        {
            await Task.Yield();

            // Apply defaults for sortBy / sortDir when empty or whitespace.
            var effectiveSortBy = string.IsNullOrWhiteSpace(sortBy) ? "name" : sortBy.Trim().ToLowerInvariant();
            var effectiveSortDir = string.IsNullOrWhiteSpace(sortDir) ? "asc" : sortDir.Trim().ToLowerInvariant();

            // Accumulate validation errors so the caller gets all problems at once.
            var errors = new List<ServiceError>();

            if (!_allowedSortBy.Contains(effectiveSortBy))
            {
                errors.Add(new ServiceError
                {
                    Code = "Validation",
                    Message = "sortBy must be one of the following values: id, name, age.",
                    Type = ServiceErrorType.Validation
                });
            }

            if (!_allowedSortDir.Contains(effectiveSortDir))
            {
                errors.Add(new ServiceError
                {
                    Code = "Validation",
                    Message = "sortDir must be either asc or desc.",
                    Type = ServiceErrorType.Validation
                });
            }

            if (pageNumber <= 0)
            {
                errors.Add(new ServiceError
                {
                    Code = "Validation",
                    Message = "Page number must be greater than 0.",
                    Type = ServiceErrorType.Validation
                });
            }

            if (pageSize <= 0 || pageSize >= 10)
            {
                errors.Add(new ServiceError
                {
                    Code = "Validation",
                    Message = "Page size must be greater than 0 and less than 10.",
                    Type = ServiceErrorType.Validation
                });
            }

            if (errors.Count > 0)
            {
                var failure = ServiceResult<PagedResponseDto<PersonDto>>.Fail(errors, 400);
                failure.ServiceErrorType = ServiceErrorType.Validation;
                return failure;
            }

            IEnumerable<PersonDto> query = _persons;

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            query = (effectiveSortBy, effectiveSortDir) switch
            {
                ("id", "asc")    => query.OrderBy(p => p.Id),
                ("id", "desc")   => query.OrderByDescending(p => p.Id),
                ("name", "asc")  => query.OrderBy(p => p.Name),
                ("name", "desc") => query.OrderByDescending(p => p.Name),
                ("age", "asc")   => query.OrderBy(p => p.Age),
                ("age", "desc") => query.OrderByDescending(p => p.Age),
                _ => query.OrderBy(p => p.Name)
            };

            var filtered = query.ToList();
            var totalCount = filtered.Count;
            var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

            var pageItems = filtered
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paged = new PagedResponseDto<PersonDto>
            {
                Data = pageItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return ServiceResult<PagedResponseDto<PersonDto>>.Ok(paged);
        }
    }
}
