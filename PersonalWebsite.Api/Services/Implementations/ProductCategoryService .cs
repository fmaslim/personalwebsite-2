using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.Products;
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

        public async Task<ServiceResult<PagedResponse<ProductCategoryDto>>> SearchCategoryAsync(string? name, int page, int pageSize, string? sortBy, string? sortDir)
        {
            var errors = new List<string>();
            IQueryable<ProductCategory> query = _context.ProductCategories.AsNoTracking();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name));
            }

            if (page <= 0)
            {
                errors.Add("Page number must be greater than 0.");
            }
            if (pageSize <= 0)
            {
                errors.Add("Page size must be greater than 0.");
            }
            else if (pageSize > 50)
            {
                errors.Add("Page size cannot exceed 50.");
            }

            sortBy = string.IsNullOrWhiteSpace(sortBy) ? "id" : sortBy.Trim().ToLower();
            sortDir = string.IsNullOrWhiteSpace(sortDir) ? "asc" : sortDir.Trim().ToLower();

            var allowedSortBy = new[] { "id", "name" };
            var allowedSortDir = new[] { "asc", "desc" };

            if (!allowedSortBy.Contains(sortBy))
            {
                errors.Add($"Invalid sortBy value. Allowed values are: {string.Join(", ", allowedSortBy)}.");
            }
            
            if (!allowedSortDir.Contains(sortDir))
            {
                errors.Add($"Invalid sortDir value. Allowed values are: {string.Join(", ", allowedSortDir)}.");
            }

            if (errors.Any())
            {
                return ServiceResult<PagedResponse<ProductCategoryDto>>.Fail(errors);
            }
            
            query = (sortBy, sortDir) switch
            {
                ("id", "asc") => query.OrderBy(c => c.ProductCategoryId),
                ("id", "desc") => query.OrderByDescending(c => c.ProductCategoryId),
                ("name", "asc") => query.OrderBy(c => c.Name),
                ("name", "desc") => query.OrderByDescending(c => c.Name),
                _ => query.OrderBy(c => c.ProductCategoryId) // default sorting
            };

            var totalCount = await query.CountAsync();
            var skip = (page - 1) * pageSize;
            var categories = await query
                .Skip(skip)
                .Take(pageSize)
                .Select(c => new ProductCategoryDto
                {
                    CategoryId = c.ProductCategoryId,
                    CategoryName = c.Name
                })
            .ToListAsync();

            var pagedResponse = new PagedResponse<ProductCategoryDto>
            {
                Items = categories,
                PageNumber = page,
                PageSize = pageSize,
                TotalRecords = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return ServiceResult<PagedResponse<ProductCategoryDto>>.Ok(pagedResponse);
        }

        public async Task<ServiceResult<ProductCategoryDto>> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _context.ProductCategories
                .AsNoTracking()
                .Where(c => c.ProductCategoryId == categoryId)
                .Select(c => new ProductCategoryDto
                {
                    CategoryId = c.ProductCategoryId,
                    CategoryName = c.Name
                })
                            .FirstOrDefaultAsync();

            if (category == null)
            {
                return ServiceResult<ProductCategoryDto>.NotFound(
                    "Category was not found",
                    "categoryId"
                    );
            }
            return ServiceResult<ProductCategoryDto>.Ok(category);
        }
    }
}
