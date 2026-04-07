using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    }
}
