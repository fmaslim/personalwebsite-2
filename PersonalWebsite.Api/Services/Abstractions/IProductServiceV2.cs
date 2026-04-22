using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IProductServiceV2
    {
        Task<GetProductByIdResultDto> GetProductByIdV2Async(int id);
    }
}
