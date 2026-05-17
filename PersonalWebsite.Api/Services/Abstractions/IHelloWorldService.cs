using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.Products;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IHelloWorldService
    {
        Task<ServiceResult<ProductDetailsDto>> GetProductByIdAsync(int id);
    }
}
