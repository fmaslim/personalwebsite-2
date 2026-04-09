using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var orderId = await _orderService.CreateOrderAsync(dto);
            return Ok(new { id = orderId });
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<OrderDetailsDto>>> SearchOrdersAsync(
            [FromQuery] int? customerId,
            [FromQuery] byte? status,
            [FromQuery] DateTime? orderDateFrom,
            [FromQuery] DateTime? orderDateTo,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery]  string? sortBy = "OrderDate",
            [FromQuery]  string? sortDir = "desc")
        {
            var orders = await _orderService.SearchOrdersAsync(customerId, status, orderDateFrom, orderDateTo, page, pageSize, sortBy, sortDir);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetailsDto?>> GetOrderByIdAsync(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        
    }
}
