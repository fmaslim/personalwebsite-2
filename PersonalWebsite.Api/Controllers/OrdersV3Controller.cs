using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.Orders;
using PersonalWebsite.Api.Models;
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllOrdersAsync([FromQuery] int? userId, 
            [FromQuery] OrderStatus? status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "orderDate",
            [FromQuery] string? sortOrder = "desc")
        {
            var result = await _orderServiceV2.GetAllOrdersAsync(userId, status, pageNumber, pageSize, sortBy, sortOrder);
            return StatusCode(result.StatusCode, result);
        }
    }
}
