using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs.Products;
using PersonalWebsite.Api.Extensions;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _prodService;
        private readonly IProductServiceV2 _productServiceV2;
        public ProductsController(IProductService productService, IProductServiceV2 productServiceV2)
        {
            _prodService = productService;
            _productServiceV2 = productServiceV2;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var result = await _prodService.GetProductListAsync();
            return result.ToActionResult();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var  result = await _prodService.GetProductByIdV2Async(id);
            return result.ToActionResult();            
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProductsAsync([FromQuery] string? name,
            [FromQuery] string? category,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortDir = null)
        {
            var result = await _prodService.SearchProductsAsync(name, category, page, pageSize, sortBy, sortDir);
            return result.ToActionResult();
        }

        [HttpGet("v2/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDetailsDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductByIdV2Async(int id)
        {
            var result = await _productServiceV2.GetProductByIdV2Async(id);

            return result.ToActionResult();
        }

        [HttpPost("v2")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateProductResponseV2Dto))]
        public async Task<IActionResult> CreateProductV2Async(CreateProductRequestV2Dto request)
        {
            var result = await _productServiceV2.CreateProductV2Async(request);
            
            return result.ToActionResult();
        }

        [HttpPut("v2")]
        [ProducesResponseType(typeof(UpdateProductResponseV2Dto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UpdateProductErrorResponseV2Dto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UpdateProductErrorResponseV2Dto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProductV2Async(UpdateProductRequestV2Dto request)
        {
            var result = await _productServiceV2.UpdateProductV2Async(request);

            return result.ToActionResult();
        }
    }
}
