using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Customers;
using PersonalWebsite.Api.Services.PerformanceTraining.Customers;

namespace PersonalWebsite.Api.Controllers.PerformanceTraining
{
    [ApiController]
    // [Route("/api/customers-v2")]
    [Route("api/performance-training/customers")]
    public class CustomersController : ApiControllerBase
    {
        private readonly ICustomerSearchTraining _service;
        private readonly ICustomerOrderSummaryService _orderSummaryService;
        public CustomersController(ICustomerSearchTraining service, ICustomerOrderSummaryService orderSummaryService)
        {
            _service = service;
            _orderSummaryService = orderSummaryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomersAsync([FromQuery]CustomerSearchRequestDto requestDto)
        {
            var result = await _service.SearchCustomersGoodQueryAsync(requestDto);
            return Ok(result);
        }

        [HttpGet("order-summary-search")]
        [ProducesResponseType(typeof(ServiceResult<PagedResponse<CustomerOrderSummaryResultDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<PagedResponse<CustomerOrderSummaryResultDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCustomerOrderSummaryAsync([FromQuery]CustomerOrderSummaryRequestDto requestDto)
        {
            var result = await _orderSummaryService.SearchCustomerOrderSummaryAsync(requestDto);
            return ToActionResult(result);
        }
    }
}
