using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/v3/orders")]
    public class OrdersV3Controller : ControllerBase
    {
        private readonly IOrderServiceV2 _orderServiceV2;

        public OrdersV3Controller(IOrderServiceV2 orderServiceV2)
        {
            _orderServiceV2 = orderServiceV2;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrderV3(CreateOrderRequestV3Dto dto)
        {
            var result = await _orderServiceV2.CreateOrderV3Async(dto);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Errors);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}
