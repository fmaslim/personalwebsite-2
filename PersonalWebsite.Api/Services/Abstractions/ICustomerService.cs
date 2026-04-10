using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
        Task<CustomerDetailsDto?> GetCustomerByIdAsync(int customerId);
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
