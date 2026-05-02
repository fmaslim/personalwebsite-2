using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using PersonalWebsite.Api.Services.PerformanceTraining.Orders;

namespace PersonalWebsite.Api.Controllers.PerformanceTraining
{
    [ApiController]
    [Route("/api/performance-training/v4/orders")]
    public class Ordersv4Controller : ApiControllerBase
    {
        private readonly IOrderv4Service _service;
        public Ordersv4Controller(IOrderv4Service service)
        {
            _service = service;
        }
        [HttpPost("{orderId}/cancel")]        
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var result = await _service.CancelOrderAsync(orderId);
            // return Ok("Order cancelled");
            return ToActionResult(result);
        }
    }
}
