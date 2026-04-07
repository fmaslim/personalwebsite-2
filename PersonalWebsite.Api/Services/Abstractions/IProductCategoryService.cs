using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IProductCategoryService
    {
        Task<IEnumerable<ProductCategoryDto>> GetAllCategoriesAsync();
    }
}
