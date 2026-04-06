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
    }
}
