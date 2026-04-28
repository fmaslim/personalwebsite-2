using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Customers;
using PersonalWebsite.Api.Services.PerformanceTraining.Customers;

namespace PersonalWebsite.Api.Controllers.PerformanceTraining
{
    [ApiController]
    // [Route("/api/customers-v2")]
    [Route("api/performance-training/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerSearchTraining _service;
        public CustomersController(ICustomerSearchTraining service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomersAsync([FromQuery]CustomerSearchRequestDto requestDto)
        {
            // var result = await _service.SearchCustomersAsync(requestDto);
            // var result = await _service.SearchCustomersBadFullEntityAsync(requestDto);
            var result = await _service.SearchCustomersBadN1QueryAsync(requestDto);
            return Ok(result);
        }
    }
}
