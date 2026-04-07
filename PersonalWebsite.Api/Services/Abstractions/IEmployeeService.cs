using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IEmployeeService
    {
        Task<EmployeeLookupDto?> GetEmployeeByIdAsync(int employeeId);
    }
}
