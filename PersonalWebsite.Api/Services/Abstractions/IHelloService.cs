using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IHelloService
    {
        Task<ServiceResult<HelloResponseDto>> GetGreetingAsync(string name);

        Task<ServiceResult<PagedResponseDto<PersonDto>>> GetPersonsAsync(
            string? name,
            string? sortBy,
            string? sortDir,
            int pageNumber,
            int pageSize);
    }
}
