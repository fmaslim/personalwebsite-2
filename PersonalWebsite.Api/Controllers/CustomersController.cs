using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.Extensions;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomersAsync()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<CustomerDetailsDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ServiceResult<CustomerDetailsDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCustomerByIdAsync(int id)
        {
            var result = await _customerService.GetCustomerByIdAsync(id);
            return result.ToActionResult();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CustomerDetailsDto>>> SearchCustomersAsync(
            [FromQuery] string? name = null,
            [FromQuery] string? accountNumber = null,
            [FromQuery] int? territoryId = null,
            [FromQuery] string? customerType = "all",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "customerId",
            [FromQuery] string? sortDir = "asc")
        {
            var customers = await _customerService.SearchCustomersAsync(name, accountNumber, territoryId, customerType, page, pageSize, sortBy, sortDir);
            return Ok(customers);
        }
    }
}
