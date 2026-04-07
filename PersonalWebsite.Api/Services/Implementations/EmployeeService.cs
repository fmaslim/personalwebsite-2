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
    }
}
