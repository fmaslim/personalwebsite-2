using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.Products;
using PersonalWebsite.Api.Extensions;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IProductCategoryService _categoryService;
        public CategoriesController(IProductCategoryService productCategoryService)
        {
            _categoryService = productCategoryService;
        }

        [HttpGet]
        public async Task<IEnumerable<ProductCategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return categories;
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(typeof(ProductCategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategoryByIdAsync(int categoryId)
        {
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);
            return result.ToActionResult();
        }
    

    [HttpGet("search")]
        public async Task<IActionResult> SearchCategoryAsync([FromQuery] string? name = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "id",
        [FromQuery] string? sortDir = "asc")
        {
            var categories = await _categoryService.SearchCategoryAsync(name, page, pageSize, sortBy, sortDir);
            return categories.ToActionResult();
        }
    }
}
