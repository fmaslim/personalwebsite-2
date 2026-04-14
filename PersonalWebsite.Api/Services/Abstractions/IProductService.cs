using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductListResponseDto> GetProductListAsync();
        Task<ProductDetailsDto?> GetProductByIdAsync(int id);
        Task<ProductListResponseDto> GetProductByNameAsync(string? name);
        Task<IEnumerable<EmployeeLookupDto>> GetEmployeeListAsync();
        //Task<IEnumerable<ProductSearchDto>> SearchProductsAsync(
        //    string? name,
        //    int page,
        //    int pageSize,
        //    string? sortBy,
        //    string? sortDir
        //    );
        Task<IEnumerable<ProductSearchDto>> SearchProductsAsync(string? name,
            string? category,
            int page = 1,
            int pageSize = 10);
    }
}
