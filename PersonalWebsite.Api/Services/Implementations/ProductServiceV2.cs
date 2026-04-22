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

        public async Task<CreateProductResultV2Dto> CreateProductV2Async(CreateProductRequestV2Dto request)
        {
            /*
             * Return 400 when:
                Name is missing
                ProductNumber is missing
                ListPrice is null
                maybe ListPrice < 0
             */
            if(string.IsNullOrWhiteSpace(request.Name))
            {
                return new CreateProductResultV2Dto
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Name is required",
                    Data = null
                };
            }
            if(string.IsNullOrWhiteSpace(request.ProductNumber))
            {
                return new CreateProductResultV2Dto
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "ProductNumber is required",
                    Data = null
                };
            }
            if(request.ListPrice == null || request.ListPrice < 0)
            {
                return new CreateProductResultV2Dto
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "ListPrice is required and must be non-negative",
                    Data = null
                };
            }
            var response = new CreateProductResponseV2Dto
            {
                Name = request.Name,
                ProductNumber = request.ProductNumber,
                ListPrice = request.ListPrice.Value
            };

            return new CreateProductResultV2Dto
            {
                Success = true,
                StatusCode = 201,
                Message = "Product created successfully",
                Data = response
            };
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
