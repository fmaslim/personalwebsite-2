using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.Products;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IProductCategoryService
    {
        Task<IEnumerable<ProductCategoryDto>> GetAllCategoriesAsync();
        // Task<ProductCategoryDto?> GetCategoryByIdAsync(int categoryId);
        Task<ServiceResult<ProductCategoryDto>> GetCategoryByIdAsync(int categoryId);
        Task<IEnumerable<ProductCategoryDto>> SearchCategoryAsync(string? name, int page, int pageSize, string? sortBy, string? sortDir);
    }
}
