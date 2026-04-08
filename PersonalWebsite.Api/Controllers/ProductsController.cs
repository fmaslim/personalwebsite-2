using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _prodService;
        public ProductsController(IProductService productService)
        {
            _prodService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            // var result = await _prodService.GetAllProductsAsync();
            var result = await _prodService.GetProductListAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDetailsDto?>> GetProductById(int id)
        {
            var result = await _prodService.GetProductByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        //[HttpGet("search")]
        //public async Task<ActionResult<ProductListResponseDto>> GetProductByName([FromQuery] string? name)
        //{
        //    var result = await _prodService.GetProductByNameAsync(name);
        //    return Ok(result);
        //}

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductSearchDto>>> SearchProductsAsync(
            [FromQuery]string? name,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortDir = null)
        {
            var result = await _prodService.SearchProductsAsync(name, page, pageSize, sortBy, sortDir);
            return Ok(result);
        }
    }
}
