using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductListResponseDto> GetProductListAsync();
        Task<ProductDetailsDto?> GetProductByIdAsync(int id);
        Task<ProductListResponseDto> GetProductByNameAsync(string? name);
    }
}
