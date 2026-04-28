using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Orders;
using PersonalWebsite.Api.Services.PerformanceTraining.Orders;

namespace PersonalWebsite.Api.Controllers.PerformanceTraining
{
    [ApiController]
    [Route("/api/performance-training")]
    public class OrderSearchTrainingController : ControllerBase
    {
        private readonly IOrderSearchTrainingService _service;
        public OrderSearchTrainingController(IOrderSearchTrainingService service)
        {
            _service = service;
        }

        [HttpGet("orders/search")]
        public async Task<IActionResult> GetSearchOrdersTrainingAsync([FromQuery]SearchOrderRequestDto dto)
        {
            var result = await _service.SearchOrdersAsync(dto);
            return Ok(result);
        }
    }
}
