using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class ProductServiceV2 : IProductServiceV2
    {
        private readonly AdventureWorksContext _context;
        public ProductServiceV2(AdventureWorksContext context)
        {
            _context = context;
        }
        public async Task<GetProductByIdResultDto> GetProductByIdV2Async(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return new GetProductByIdResultDto
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Product not found",
                    Data = null
                };
            }

            var productDetails = new ProductDetailsDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                ListPrice = product.ListPrice,
                ProductNumber = product.ProductNumber
            };

            return new GetProductByIdResultDto
            {
                Success = true,
                StatusCode = 200,
                Data = productDetails,
                Message = "Product retrieved successfully"
            };
        }
    }
}
