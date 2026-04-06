using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly AdventureWorksContext _context;
        public ProductService(AdventureWorksContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
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
            var query = _context.Products.AsNoTracking().AsQueryable();
            query = query.OrderBy(p => p.Name);
            query = query.Take(25);
            var products = await query.Select(p => new ProductListDto { 
                ProductId = p.ProductId,
                Name = p.Name,
                ListPrice = p.ListPrice
            }).ToListAsync();

            //return products;
            var count = products.Count;
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

            var query = _context.Products.AsNoTracking().AsQueryable();
            query = query.Where(p => p.ProductId == id);
            var productDetailDto = await query.Select(p => new ProductDetailsDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                ProductNumber = p.ProductNumber,
                ListPrice = p.ListPrice
            }).FirstOrDefaultAsync();

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
    }
}
