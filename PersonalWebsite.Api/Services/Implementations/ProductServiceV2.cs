using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.Products;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Models.Errors;
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

        //public Task<CreateProductResultV2Dto> CreateProductV2Async(CreateProductRequestV2Dto request)
        //{
        //    /*
        //     * Return 400 when:
        //        Name is missing
        //        ProductNumber is missing
        //        ListPrice is null
        //        maybe ListPrice < 0
        //     */
        //    if(string.IsNullOrWhiteSpace(request.Name))
        //    {
        //        return Task.FromResult(new CreateProductResultV2Dto
        //        {
        //            Success = false,
        //            StatusCode = 400,
        //            Message = "Name is required",
        //            Data = null
        //        });
        //    }
        //    if(string.IsNullOrWhiteSpace(request.ProductNumber))
        //    {
        //        return Task.FromResult(new CreateProductResultV2Dto
        //        {
        //            Success = false,
        //            StatusCode = 400,
        //            Message = "ProductNumber is required",
        //            Data = null
        //        });
        //    }
        //    if(request.ListPrice == null || request.ListPrice < 0)
        //    {
        //        return Task.FromResult(new CreateProductResultV2Dto
        //        {
        //            Success = false,
        //            StatusCode = 400,
        //            Message = "ListPrice is required and must be non-negative",
        //            Data = null
        //        });
        //    }
        //    var response = new CreateProductResponseV2Dto
        //    {
        //        Name = request.Name,
        //        ProductNumber = request.ProductNumber,
        //        ListPrice = request.ListPrice.Value
        //    };

        //    return Task.FromResult(new CreateProductResultV2Dto
        //    {
        //        Success = true,
        //        StatusCode = 201,
        //        Message = "Product created successfully",
        //        Data = response
        //    });
        //}

        public async Task<ServiceResult<ProductDetailsDto>> GetProductByIdV2Async(int id)
        {
            if (id <= 0)
            {
                return ServiceResult<ProductDetailsDto>.Fail(
                "Product ID must be greater than 0.",
                ServiceErrorType.Validation);
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return ServiceResult<ProductDetailsDto>.NotFound("Product was not found");
            }

            var productDetails = new ProductDetailsDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                ListPrice = product.ListPrice,
                ProductNumber = product.ProductNumber
            };
            return ServiceResult<ProductDetailsDto>.Ok(productDetails, "Product retrieved successfully");
        }

        public async Task<ServiceResult<UpdateProductResultV2Dto>> UpdateProductV2Async(UpdateProductRequestV2Dto request)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == request.ProductId);
            if (product == null)
            {
                return ServiceResult<UpdateProductResultV2Dto>.NotFound("Product not found");
            }
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.ProductNumber))
            {
                return ServiceResult<UpdateProductResultV2Dto>.Fail("Name and ProductNumber are required", ServiceErrorType.Validation);                
            }
            if (request.ListPrice == null)
            {
                return ServiceResult<UpdateProductResultV2Dto>.Fail("ListPrice is required", ServiceErrorType.Validation);                
            }

            if (request.ListPrice.Value < 0)
            {
                return ServiceResult<UpdateProductResultV2Dto>.Fail("ListPrice cannot be negative", ServiceErrorType.Validation);
            }

            product.Name = request.Name;
            product.ProductNumber = request.ProductNumber;
            product.ListPrice = request.ListPrice.Value;

            await _context.SaveChangesAsync();
                        
            var result =  new UpdateProductResultV2Dto
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
            return ServiceResult<UpdateProductResultV2Dto>.Ok(result);
        }

        public Task<ServiceResult<CreateProductResultV2Dto>> CreateProductV2Async(CreateProductRequestV2Dto request)
        {
            if (request == null)
            {
                return Task.FromResult(
                    ServiceResult<CreateProductResultV2Dto>.Fail(
                        "Request object is required",
                        ServiceErrorType.Validation));
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Task.FromResult(
                    ServiceResult<CreateProductResultV2Dto>.Fail(
                        "Name is required",
                        ServiceErrorType.Validation));
            }

            if (string.IsNullOrWhiteSpace(request.ProductNumber))
            {
                return Task.FromResult(
                    ServiceResult<CreateProductResultV2Dto>.Fail(
                        "ProductNumber is required",
                        ServiceErrorType.Validation));
            }

            if (request.ListPrice == null)
            {
                return Task.FromResult(
                    ServiceResult<CreateProductResultV2Dto>.Fail(
                        "ListPrice is required",
                        ServiceErrorType.Validation));
            }

            if (request.ListPrice < 0)
            {
                return Task.FromResult(
                    ServiceResult<CreateProductResultV2Dto>.Fail(
                        "ListPrice cannot be negative",
                        ServiceErrorType.Validation));
            }

            var response = new CreateProductResponseV2Dto
            {
                Name = request.Name,
                ProductNumber = request.ProductNumber,
                ListPrice = request.ListPrice ?? 0
            };

            var result = new CreateProductResultV2Dto
            {
                Success = true,
                StatusCode = 201,
                Message = "Product created successfully",
                Data = response
            };

            return Task.FromResult(ServiceResult<CreateProductResultV2Dto>.Ok(result));
        }
    }
}
