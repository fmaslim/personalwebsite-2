using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.Orders;
using PersonalWebsite.Api.Extensions;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status409Conflict)]
        //public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        //{
        //    var result = await _orderService.CreateOrderAsync(dto);
        //    return result.ToActionResult();
        //    //var result = await _orderService.CreateOrderV2Async(dto);
        //    //return result.ToActionResult();
        //}

        [HttpPost]
        public async Task<IActionResult> CreateOrderV2([FromBody] CreateOrderRequestDto dto)
        {
            var result = await _orderService.CreateOrderV2Async(dto);
            return result.ToActionResult();
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchOrdersAsync(
            [FromQuery] int? customerId,
            [FromQuery] byte? status,
            [FromQuery] DateTime? orderDateFrom,
            [FromQuery] DateTime? orderDateTo,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "OrderDate",
            [FromQuery] string? sortDir = "desc")
        {
            var orders = await _orderService.SearchOrdersAsync(customerId, status, orderDateFrom, orderDateTo, page, pageSize, sortBy, sortDir);

            return orders.ToActionResult();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderByIdAsync(int id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);

            return result.ToActionResult();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderAsync(int id, [FromBody] UpdateOrderRequestDto dto)
        {
            var result = await _orderService.UpdateOrderAsync(id, dto);
            return result.ToActionResult();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteOrderAsync(int id)
        {
            var result = await _orderService.DeleteOrderAsync(id);
            return result.ToActionResult();
        }
    }
}
