using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AdventureWorksContext _context;
        public EmployeeService(AdventureWorksContext context)
        {
            _context = context;
        }
        public async Task<EmployeeLookupDto?> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = _context.Employees
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

            return await employee;
        }

        public async Task<IEnumerable<EmployeeLookupDto>> SearchEmployeesAsync(
            // string? name, // skip this for now since it requires concatenation of first and last name which is a bit more complex to do efficiently in EF Core
            string? jobTitle, 
            bool? currentFlag, 
            int page, 
            int pageSize, 
            string? sortBy, 
            string? sortDir)
        {
            // 1. start query
            var query = _context.Employees
                .AsNoTracking()
                .AsQueryable();

            // 2. normalize params
            // name = name?.Trim().ToLower();
            jobTitle = jobTitle?.Trim().ToLower();
            sortBy = sortBy?.Trim().ToLower();
            sortDir = sortDir?.Trim().ToLower();

            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 50 ? 50 : pageSize;

            bool desc = sortDir == "desc";

            // skip name for now since it requires concatenation of first and last name which is a bit more complex to do efficiently in EF Core
            //if(!string.IsNullOrEmpty(name))
            //{
            //    query = query.Where(e => (e.FirstName + " " + e.LastName).ToLower().Contains(name));
            //}
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
            if(sortBy == "jobTitle")
            {
                query = desc ? query.OrderByDescending(e => e.JobTitle) : query.OrderBy(e => e.JobTitle);
            }
            else if(sortBy == "hiredate")
            {
                query = desc ? query.OrderByDescending(e => e.HireDate) : query.OrderBy(e => e.HireDate);
            }
            else
            {
                // default sort by employee id
                query = desc ? query.OrderByDescending(e => e.BusinessEntityId) : query.OrderBy(e => e.BusinessEntityId);
            }

            // 3. apply pagination
            var employees = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EmployeeLookupDto
                {
                    EmployeeId = e.BusinessEntityId,
                    JobTitle = e.JobTitle,
                    HireDate = e.HireDate,
                    CurrentFlag = e.CurrentFlag
                })
                .ToListAsync();

            return employees;
        }
    }
}
