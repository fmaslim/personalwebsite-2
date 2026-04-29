using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.Orders;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Orders;
using PersonalWebsite.Api.Services.Abstractions;
using PersonalWebsite.Api.Services.PerformanceTraining.Orders;

namespace PersonalWebsite.Api.Controllers.PerformanceTraining
{
    [ApiController]
    [Route("/api/performance-training")]
    public class OrderSearchTrainingController : ControllerBase
    {
        private readonly IOrderSearchTrainingService _service;
        private readonly IOrderService _orderService;
        public OrderSearchTrainingController(IOrderSearchTrainingService service, IOrderService orderService)
        {
            _service = service;
            _orderService = orderService;
        }

        [HttpGet("orders/search")]
        public async Task<IActionResult> GetSearchOrdersTrainingAsync([FromQuery]SearchOrderRequestDto dto)
        {
            var result = await _service.SearchOrdersAsync(dto);
            return Ok(result);
        }

        [HttpGet("orders/search-badn1query")]
        public async Task<IActionResult> GetOrdersBadN1Async([FromQuery] DTOs.PerformanceTraining.OrderSearchRequestDto requestDto)
        {
            var result = await _orderService.SearchOrdersBadN1QueryAsync(requestDto);
            return Ok(result);
        }
    }
}
