using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IEmployeeService
    {
        Task<EmployeeLookupDto?> GetEmployeeByIdAsync(int employeeId);
        Task<IEnumerable<EmployeeLookupDto>> SearchEmployeesAsync(
            string? name, // skip this for now since it requires concatenation of first and last name which is a bit more complex to do efficiently in EF Core
            string? jobTitle, 
            bool? currentFlag,
            int page,
            int pageSize,
            string? sortBy,
            string? sortDir);
    }
}
