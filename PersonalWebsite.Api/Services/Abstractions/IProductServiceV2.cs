using PersonalWebsite.Api.DTOs.Products;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IProductServiceV2
    {
        Task<GetProductByIdResultDto> GetProductByIdV2Async(int id);
        Task<CreateProductResultV2Dto> CreateProductV2Async(CreateProductRequestV2Dto request);
        Task<UpdateProductResultV2Dto> UpdateProductV2Async(UpdateProductRequestV2Dto request);
    }
}
