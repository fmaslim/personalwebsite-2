using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly AdventureWorksContext _context;
        private readonly ILogger<ProductService> _logger;
        public ProductService(AdventureWorksContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            _logger.LogInformation("Getting products");

            var products = await _context.Products
                .OrderBy(p => p.ProductId)
                .Take(25)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    ListPrice = p.ListPrice
                })
                .ToListAsync();

            _logger.LogInformation("Returned {ProductCount} products", products.Count);
            return products;
        }

        public async Task<ProductListResponseDto> GetProductListAsync()
        {
            /*
             * Requirements

The endpoint should:

read from Products
sort by Name
return only first 25
project only needed columns into DTO
not return full entity
use async
             */

            _logger.LogInformation("GetProductListAsync");

            var query = _context.Products.AsNoTracking().AsQueryable();
            query = query.OrderBy(p => p.Name);
            query = query.Take(25);
            var products = await query.Select(p => new ProductListDto { 
                ProductId = p.ProductId,
                Name = p.Name,
                ListPrice = p.ListPrice
            }).ToListAsync();
                        
            var count = products.Count;
            _logger.LogInformation("Returned {ProductCount} products", products.Count);

            var response = new ProductListResponseDto
            {
                Count = count,
                Items = products
            };

            return response;
        }

        public async Task<ProductDetailsDto?> GetProductByIdAsync(int id)
        {
            /*
             * Requirements
filter by id
project to DTO
async
return NotFound() if no product exists
             */
            _logger.LogInformation("Getting product by id {ProductId}", id);

            var query = _context.Products.AsNoTracking();
            query = query.Where(p => p.ProductId == id);
            var productDetailDto = await query.Select(p => new ProductDetailsDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                ProductNumber = p.ProductNumber,
                ListPrice = p.ListPrice
            }).FirstOrDefaultAsync();

            if (productDetailDto == null)
            {
                _logger.LogWarning("Product with id {ProductId} not found", id);
                return null;
            }

            return productDetailDto;
        }

        public async Task<ProductListResponseDto> GetProductByNameAsync(string? name)
        {
            var query = _context.Products.AsNoTracking();
            if (string.IsNullOrWhiteSpace(name))
            {
                return new ProductListResponseDto
                {
                    Count = 0,
                    Items = new List<ProductListDto>()
                };
            }
            query = query.Where(p => p.Name.Contains(name));

            var count = await query.CountAsync();
            var products = await query.OrderBy(p => p.Name)
                .Take(25)
                .Select(p => new ProductListDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    ListPrice = p.ListPrice
                }).ToListAsync();

            var productListResponse = new ProductListResponseDto
            {
                Count = count,
                Items = products
            };

            return productListResponse;
        }

        public async Task<IEnumerable<EmployeeLookupDto>> GetEmployeeListAsync()
        {
            var query = _context.Employees
            .AsNoTracking()
            .Join(
                _context.People.AsNoTracking(),
                e => e.BusinessEntityId,
                p => p.BusinessEntityId,
                (e, p) => new EmployeeLookupDto
                {
                    EmployeeId = e.BusinessEntityId,
                    FullName = p.FirstName + " " + p.LastName
                })
            .OrderBy(x => x.FullName);

                    var employees = await query.ToListAsync();

            return employees;
        }

        public async Task<IEnumerable<ProductSearchDto>> SearchProductsAsync(string? name, string? category, int page = 1, int pageSize = 10, string? sortBy = null, string? sortDir = null)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();

            // filter by name
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            // filter by category
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.ProductSubcategory != null && p.ProductSubcategory.ProductCategory != null && p.ProductSubcategory.ProductCategory.Name.Contains(category));
            }

            // sorting
            var sortByNormalized = sortBy?.Trim().ToLower();
            var sortDirNormalized = sortDir?.Trim().ToLower();
            query = (sortByNormalized, sortDirNormalized) switch
            {
                ("name", "asc") => query.OrderBy(p => p.Name),
                ("name", "desc") => query.OrderByDescending(p => p.Name),
                ("listprice", "asc") => query.OrderBy(p => p.ListPrice),
                ("listprice", "desc") => query.OrderByDescending(p => p.ListPrice),
                ("id", "asc") => query.OrderBy(p => p.ProductId),
                ("id", "desc") => query.OrderByDescending(p => p.ProductId),
                _ => query.OrderBy(p => p.ProductId)
            };

            // pagination
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            var skip = (page - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);            

            var products = query.Select(p => new ProductSearchDto
            {
                ProductId = p.ProductId,
                ProductName = p.Name,
                ProductNumber = p.ProductNumber,
                ListPrice = p.ListPrice
            }).ToListAsync();

            return await products;
        }

        //public async Task<IEnumerable<ProductSearchDto>> SearchProductsAsync(string? name, int page, int pageSize, string? sortBy, string? sortDir)
        //{
        //    var query = _context.Products.AsNoTracking();

        //    // filter by name
        //    if (!String.IsNullOrWhiteSpace(name))
        //    {
        //        query = query.Where(p => p.Name.Contains(name));
        //    }

        //    var sortByNormalized = sortBy?.Trim().ToLower();
        //    var sortDirNormalized = sortDir?.Trim().ToLower();

        //    // sorting
        //    query = (sortByNormalized, sortDirNormalized) switch
        //    {
        //        ("name", "asc") => query.OrderBy(p => p.Name),
        //        ("name", "desc") => query.OrderByDescending(p => p.Name),
        //        ("listprice", "asc") => query.OrderBy(p => p.ListPrice),
        //        ("listprice", "desc") => query.OrderByDescending(p => p.ListPrice),
        //        ("id", "asc") => query.OrderBy(p => p.ProductId),
        //        ("id", "desc") => query.OrderByDescending(p => p.ProductId),
        //        _ => query.OrderBy(p => p.ProductId)
        //    };

        //    // pagination
        //    if (page < 1) page = 1;
        //    if (pageSize < 1) pageSize = 10;
        //    var skip = (page - 1) * pageSize;
        //    query = query.Skip(skip).Take(pageSize);

        //    var products = query.Select(p => new ProductSearchDto
        //    {
        //        ProductId = p.ProductId,
        //        ProductName = p.Name,
        //        ProductNumber = p.ProductNumber,
        //        ListPrice = p.ListPrice
        //    }).ToListAsync();

        //    return await products;
        //}
    }
}
