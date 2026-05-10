using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.Products;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IProductServiceV2
    {
        Task<ServiceResult<ProductDetailsDto>> GetProductByIdV2Async(int id);
        Task<ServiceResult<CreateProductResponseV2Dto>> CreateProductV2Async(CreateProductRequestV2Dto request);
        Task<ServiceResult<UpdateProductResultV2Dto>> UpdateProductV2Async(UpdateProductRequestV2Dto request);        
    }
}
