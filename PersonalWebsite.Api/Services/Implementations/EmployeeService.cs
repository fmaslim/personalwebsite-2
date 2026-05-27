using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AdventureWorksContext _context;
        public EmployeeService(AdventureWorksContext context)
        {
            _context = context;
        }        

        public async Task<ServiceResult<EmployeeLookupDto>> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .Where(e => e.BusinessEntityId == employeeId)
                .Select(e => new EmployeeLookupDto
                {
                    EmployeeId = e.BusinessEntityId,
                    JobTitle = e.JobTitle,
                    HireDate = e.HireDate,
                    CurrentFlag = e.CurrentFlag
                })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                return ServiceResult<EmployeeLookupDto>.NotFound(
                    "Employee Not found"
                    );
            }

            return ServiceResult<EmployeeLookupDto>.Ok(employee);
        }

        public async Task<ServiceResult<PagedResponse<EmployeeLookupDto>>> SearchEmployeesAsync(
            string? name, // skip this for now since it requires concatenation of first and last name which is a bit more complex to do efficiently in EF Core
            string? jobTitle, 
            bool? currentFlag, 
            int page, 
            int pageSize, 
            string? sortBy, 
            string? sortDir)
        {
            var errors = new List<string>();

            // 1. start query
            var query = _context.Employees
                .AsNoTracking()
                .AsQueryable();

            // Add paging validations
            if (page <= 0)
            {
                errors.Add("Page number must be greater than 0.");
            }
            if (pageSize <= 0)
            {
                errors.Add("Page size must be greater than 0.");
            }
            else if (pageSize > 50)
            {
                errors.Add("Page size cannot be greater than 50.");
            }

            sortBy = string.IsNullOrWhiteSpace(sortBy) ? "id" : sortBy.Trim().ToLower();
            sortDir = string.IsNullOrWhiteSpace(sortDir) ? "asc" : sortDir.Trim().ToLower();

            var allowedSortBy = new[]
            {
                "id",
                "jobtitle",
                "currentflag",
                "hiredate",
                "fullname"
            };
            var allowedSortDir = new[] { "asc", "desc" };
            if (!allowedSortBy.Contains(sortBy))
            {
                errors.Add($"Invalid sortBy value. Allowed values are: {string.Join(", ", allowedSortBy)}.");
            }
            if (!allowedSortDir.Contains(sortDir))
            {
                errors.Add($"Invalid sortDir value. Allowed values are: {string.Join(", ", allowedSortDir)}.");
            }
            if (errors.Any())
            {
                return ServiceResult<PagedResponse<EmployeeLookupDto>>.Fail(errors);
            }

            bool desc = sortDir == "desc";
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => (e.BusinessEntity.FirstName + " " + e.BusinessEntity.LastName).ToLower().Contains(name)
                || e.BusinessEntity.FirstName.ToLower().Contains(name)
                || e.BusinessEntity.LastName.ToLower().Contains(name));
            }
            if (!string.IsNullOrEmpty(jobTitle))
            {
                query = query.Where(e => e.JobTitle.ToLower().Contains(jobTitle));
                // filter first, then sort after to ensure sorting is applied to the filtered results
                // query = desc ? query.OrderByDescending(e => e.JobTitle) : query.OrderBy(e => e.JobTitle);
            }
            if(currentFlag.HasValue)
            {
                query = query.Where(e => e.CurrentFlag == currentFlag.Value);
                // filter first, then sort after to ensure sorting is applied to the filtered results
                // query = desc ? query.OrderByDescending(e => e.CurrentFlag) : query.OrderBy(e => e.CurrentFlag);
            }
            if (sortBy == "jobtitle")
            {
                query = desc ? query.OrderByDescending(e => e.JobTitle) : query.OrderBy(e => e.JobTitle);
            }
            else if (sortBy == "hiredate")
            {
                query = desc ? query.OrderByDescending(e => e.HireDate) : query.OrderBy(e => e.HireDate);
            }
            else if (sortBy == "fullname")
            {
                query = desc ? query.OrderByDescending(e => e.BusinessEntity.LastName).ThenByDescending(e => e.BusinessEntity.FirstName)
                    : query.OrderBy(e => e.BusinessEntity.LastName).ThenBy(e => e.BusinessEntity.FirstName);
            }
            else if (sortBy == "currentflag")
            {
                query = desc ? query.OrderByDescending(e => e.CurrentFlag) : query.OrderBy(e => e.CurrentFlag);
            }
            else
            {
                // default sort by employee id
                query = desc ? query.OrderByDescending(e => e.BusinessEntityId) : query.OrderBy(e => e.BusinessEntityId);
            }

            // Get totalcount before pagination is applied
            var totalCount = await query.CountAsync();
            var skip = (page - 1) * pageSize;

            // 3. apply pagination
            var employees = await query
                .Skip(skip)
                .Take(pageSize)
                .Select(e => new EmployeeLookupDto
                {
                    EmployeeId = e.BusinessEntityId,
                    FullName = e.BusinessEntity.FirstName + " " + e.BusinessEntity.LastName,
                    JobTitle = e.JobTitle,
                    HireDate = e.HireDate,
                    CurrentFlag = e.CurrentFlag
                })
                .ToListAsync();

            var pagedResponse = new PagedResponse<EmployeeLookupDto>
            {
                Items = employees,
                PageNumber = page,
                PageSize = pageSize,
                TotalRecords = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return ServiceResult<PagedResponse<EmployeeLookupDto>>.Ok(pagedResponse);
        }
    }
}
