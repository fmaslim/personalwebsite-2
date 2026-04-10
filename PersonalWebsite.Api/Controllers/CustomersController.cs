using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
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
        public async Task<ActionResult<CustomerDetailsDto?>> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CustomerDetailsDto>>> SearchCustomersAsync(
            [FromQuery] string? name = null,
            [FromQuery] string? accountNumber = null,
            [FromQuery] int? territoryId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "customerId",
            [FromQuery] string? sortDir = "asc")
        {
            var customers = await _customerService.SearchCustomersAsync(name, accountNumber, territoryId, page, pageSize, sortBy, sortDir);
            return Ok(customers);
        }
    }
}
