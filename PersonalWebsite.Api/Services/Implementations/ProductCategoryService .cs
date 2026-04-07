using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly AdventureWorksContext _context;
        public ProductCategoryService(AdventureWorksContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ProductCategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.ProductCategories
        .AsNoTracking()
        .OrderBy(c => c.Name)
        .Select(c => new ProductCategoryDto
        {
            CategoryId = c.ProductCategoryId,
            CategoryName = c.Name
        })
        .ToListAsync();

            return categories;
        }

        public async Task<ProductCategoryDto?> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _context.ProductCategories
                .AsNoTracking()
                .Where(c => c.ProductCategoryId == categoryId)
                .Select(c => new ProductCategoryDto { 
                    CategoryId = c.ProductCategoryId,
                    CategoryName = c.Name
                })
                .FirstOrDefaultAsync();

            return category;
        }
    }
}
