using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.Common;
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
        private readonly ICustomerOrderSummaryService _orderSummaryService;
        public CustomersController(ICustomerSearchTraining service, ICustomerOrderSummaryService orderSummaryService)
        {
            _service = service;
            _orderSummaryService = orderSummaryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomersAsync([FromQuery]CustomerSearchRequestDto requestDto)
        {
            // var result = await _service.SearchCustomersAsync(requestDto);
            // var result = await _service.SearchCustomersBadFullEntityAsync(requestDto);
            // var result = await _service.SearchCustomersBadN1QueryAsync(requestDto);
            var result = await _service.SearchCustomersGoodQueryAsync(requestDto);
            return Ok(result);
        }

        [HttpGet("order-summary-search")]
        [ProducesResponseType(typeof(ServiceResult<PagedResponse<CustomerOrderSummaryResultDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<PagedResponse<CustomerOrderSummaryResultDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCustomerOrderSummaryAsync([FromQuery]CustomerOrderSummaryRequestDto requestDto)
        {
            var result = await _orderSummaryService.SearchCustomerOrderSummaryAsync(requestDto);
            // return StatusCode(result.StatusCode, result);
            return ToActionResult(result);
        }

        private IActionResult ToActionResult<T>(ServiceResult<T> result)
        {
            if (result.Success)
            {
                return Ok(result);
            }

            var errorType = result.Errors.FirstOrDefault()?.Type;

            return errorType switch
            { 
                Models.Errors.ServiceErrorType.Validation => BadRequest(result),
                Models.Errors.ServiceErrorType.NotFound => NotFound(result),
                Models.Errors.ServiceErrorType.Conflict => Conflict(result),
                _ => StatusCode(result.StatusCode, result)
            }
            ;
        }
    }
}
