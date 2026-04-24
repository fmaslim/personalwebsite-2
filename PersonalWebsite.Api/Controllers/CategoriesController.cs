using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.Products;
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
        public async Task<ActionResult<ProductCategoryDto?>> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }
    

    [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductCategoryDto>>> SearchCategoryAsync(string? name = null,
        int page = 1,
        int pageSize = 10,
        string? sortBy = "categoryid",
        string? sortDir = "asc")
        {
            var categories = await _categoryService.SearchCategoryAsync(name, page, pageSize, sortBy, sortDir);
            return Ok(categories);
        }
    }
}
