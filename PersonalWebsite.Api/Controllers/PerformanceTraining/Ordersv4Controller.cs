using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.Extensions;
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
        /// <summary>
        /// Cancels an order if the order is eligible to be cancelled.
        /// </summary>
        /// <param name="orderId">The order id to cancel.</param>
        /// <returns>The cancellation result.</returns>
        [HttpPost("{orderId}/cancel")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ServiceResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ServiceResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ServiceResult<string>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var result = await _service.CancelOrderAsync(orderId);

            //if (!result.Success)
            //{
            //    return BadRequest(result);
            //}

            //return Ok(result.Data);
            return result.ToActionResult();
        }
    }
}
