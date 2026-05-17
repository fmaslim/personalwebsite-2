using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.Products;
using PersonalWebsite.Api.Extensions;
using PersonalWebsite.Api.Services.Abstractions;
using Microsoft.AspNetCore.Http;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/hello")]
    public class HelloWorldController : ControllerBase
    {
        private readonly IHelloWorldService _helloService;

        public HelloWorldController(IHelloWorldService helloService)
        {
            _helloService = helloService;
        }

        [HttpGet]
        public IActionResult GetHello()
        {
            return Ok("Hello, world!");
        }

        [HttpGet("service")]
        public IActionResult GetHelloService()
        {
            var result = ServiceResult<string>.Ok("Hello, world!");
            return result.ToActionResult();
        }

        [HttpGet("product/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDetailsDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _helloService.GetProductByIdAsync(id);
            return result.ToActionResult();
        }
    }
}
