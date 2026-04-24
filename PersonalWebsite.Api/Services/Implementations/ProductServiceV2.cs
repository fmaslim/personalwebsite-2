using PersonalWebsite.Api.DTOs.Products;
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

        public Task<CreateProductResultV2Dto> CreateProductV2Async(CreateProductRequestV2Dto request)
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
                return Task.FromResult(new CreateProductResultV2Dto
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Name is required",
                    Data = null
                });
            }
            if(string.IsNullOrWhiteSpace(request.ProductNumber))
            {
                return Task.FromResult(new CreateProductResultV2Dto
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "ProductNumber is required",
                    Data = null
                });
            }
            if(request.ListPrice == null || request.ListPrice < 0)
            {
                return Task.FromResult(new CreateProductResultV2Dto
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "ListPrice is required and must be non-negative",
                    Data = null
                });
            }
            var response = new CreateProductResponseV2Dto
            {
                Name = request.Name,
                ProductNumber = request.ProductNumber,
                ListPrice = request.ListPrice.Value
            };

            return Task.FromResult(new CreateProductResultV2Dto
            {
                Success = true,
                StatusCode = 201,
                Message = "Product created successfully",
                Data = response
            });
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

        public async Task<UpdateProductResultV2Dto> UpdateProductV2Async(UpdateProductRequestV2Dto request)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == request.ProductId);
            if (product == null)
            {
                return new UpdateProductResultV2Dto
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Product with id {request.ProductId} not found"
                };
            }
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.ProductNumber))
            {
                return new UpdateProductResultV2Dto
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Name and ProductNumber are required"
                };
            }
            if (request.ListPrice == null)
            {
                return new UpdateProductResultV2Dto
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "ListPrice is required"
                };
            }

            if (request.ListPrice.Value < 0)
            {
                return new UpdateProductResultV2Dto
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "ListPrice cannot be negative"
                };
            }

            product.Name = request.Name;
            product.ProductNumber = request.ProductNumber;
            product.ListPrice = request.ListPrice.Value;

            await _context.SaveChangesAsync();

            return new UpdateProductResultV2Dto
            {
                Success = true,
                StatusCode = 200,
                Message = "Product updated successfully",
                Data = new UpdateProductResponseV2Dto
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    ProductNumber = product.ProductNumber,
                    ListPrice = product.ListPrice
                }
            };
        }
    }
}
