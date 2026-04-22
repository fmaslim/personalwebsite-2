using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IProductServiceV2
    {
        Task<GetProductByIdResultDto> GetProductByIdV2Async(int id);
        Task<CreateProductResultV2Dto> CreateProductV2Async(CreateProductRequestV2Dto request);
    }
}
