using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.Models.Errors;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
        // Task<CustomerDetailsDto?> GetCustomerByIdAsync(int customerId);
        Task<ServiceResult<CustomerDetailsDto>> GetCustomerByIdAsync(int customerId);        
        Task<IEnumerable<CustomerDetailsDto>> SearchCustomersAsync(string? name,
            string? accountNumber,
            int? territoryId,
            string? customerType,
            int page,
            int pageSize,
            string? sortBy,
            string? sortDir
            );
    }
}
