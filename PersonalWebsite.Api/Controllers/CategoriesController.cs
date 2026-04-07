using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
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
    }
}
