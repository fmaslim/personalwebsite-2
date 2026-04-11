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

        public async Task<IEnumerable<ProductCategoryDto>> SearchCategoryAsync(string? name, int page, int pageSize, string? sortBy, string? sortDir)
        {
            var query = _context.ProductCategories.AsNoTracking();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name));
            }

            page = page <= 0 ? 1 : page;            
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 50 ? 50 : pageSize; // pageSize capped at 50

            // normalize sort parameters
            sortBy = sortBy?.Trim().ToLower();
            sortDir = sortDir?.Trim().ToLower();

            bool desc = sortDir == "desc";

            // apply sorting
            if (sortBy == "categoryid")
            {
                query = desc ? query.OrderByDescending(c => c.ProductCategoryId) : query.OrderBy(c => c.ProductCategoryId);
            }
            else
            {
                query = desc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name);
            }

            // apply pagination
            var skip = (page - 1) * pageSize;
            return await query
            .Skip(skip)
            .Take(pageSize)
            .Select(c => new ProductCategoryDto
            {
                CategoryId = c.ProductCategoryId,
                CategoryName = c.Name
            })
            .ToListAsync();


        }
    }
}
