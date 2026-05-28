using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.Extensions;
using PersonalWebsite.Api.Services.Abstractions;
using System.Diagnostics.Contracts;

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
        [ProducesResponseType(typeof(ServiceResult<PagedResponse<CustomerDetailsDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<PagedResponse<CustomerDetailsDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchCustomersAsync(
            [FromQuery] string? name = null,
            [FromQuery] string? accountNumber = null,
            [FromQuery] int? territoryId = null,
            [FromQuery] string? customerType = "all",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortDir = null)
        {
            var customers = await _customerService.SearchCustomersAsync(name, accountNumber, territoryId, customerType, page, pageSize, sortBy, sortDir);
            return customers.ToActionResult();
        }

        [HttpGet("{customerId}/orders")]
        public async Task<IActionResult> GetCustomerOrdersAsync(int customerId,
            int pageNumber = 1,
            int pageSize = 10,
            string? sortBy = "orderDate",
            string? sortDir = "desc",
            string? status = "",
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var result = await _customerService.GetCustomerOrdersAsync(customerId,
                pageNumber,
                pageSize,
                sortBy,
                sortDir,
                status,
                fromDate,
                toDate);
            return result.ToActionResult();
        }

        [HttpGet("test-crash")]
        public IActionResult TestCrash()
        {
                       throw new Exception("This is a test exception for crash testing.");
        }
    }
}
