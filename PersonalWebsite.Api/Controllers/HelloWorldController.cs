using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.Products;
using PersonalWebsite.Api.DTOs;
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
        private readonly IHelloService _greetingService;

        public HelloWorldController(IHelloWorldService helloService, IHelloService greetingService)
        {
            _helloService = helloService;
            _greetingService = greetingService;
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

        [HttpGet("persons")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponseDto<PersonDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPersons(
            [FromQuery] string? name,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortDir,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5)
        {
            var result = await _greetingService.GetPersonsAsync(name, sortBy, sortDir, pageNumber, pageSize);
            return result.ToActionResult();
        }

        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HelloResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetGreeting(string name)
        {
            var result = await _greetingService.GetGreetingAsync(name);
            return result.ToActionResult();
        }
    }
}
