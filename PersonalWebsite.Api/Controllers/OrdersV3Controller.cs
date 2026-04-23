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

        [HttpPatch("{orderId}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequestDto dto)
        {
            var result = await _orderServiceV2.UpdateOrderStatusAsync(orderId, dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{orderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderByIdAsync(int orderId)
        {
            var result = await _orderServiceV2.GetOrderByIdAsync(orderId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
