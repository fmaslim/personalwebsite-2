using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.Products;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class HelloWorldService : IHelloWorldService
    {
        private readonly AdventureWorksContext _context;
        private readonly ILogger<HelloWorldService> _logger;

        public HelloWorldService(AdventureWorksContext context, ILogger<HelloWorldService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<ProductDetailsDto>> GetProductByIdAsync(int id)
        {
            if (id <= 0)
            {
                return ServiceResult<ProductDetailsDto>.Fail(
                    "ProductId must be greater than 0.",
                    Models.Errors.ServiceErrorType.Validation);
            }

            var product = await _context.Products
                .AsNoTracking()
                .Where(p => p.ProductId == id)
                .Select(p => new ProductDetailsDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    ProductNumber = p.ProductNumber,
                    ListPrice = p.ListPrice
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return ServiceResult<ProductDetailsDto>.NotFound($"Product with ID {id} does not exist.", "ProductId");
            }

            return ServiceResult<ProductDetailsDto>.Ok(product);
        }
    }
}
