using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Services.Abstractions;
using Microsoft.AspNetCore.Http;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/v2/orders")]
    public class OrdersV2Controller : ControllerBase
    {
        private readonly IOrderServiceV2 _orderServiceV2;
        public OrdersV2Controller(IOrderServiceV2 orderServiceV2)
        {
            _orderServiceV2 = orderServiceV2;
        }
        [HttpGet]
        [ProducesResponseType(statusCode: 200)]
        public IActionResult GetOrder()
        {
            return Ok(new { Message = "This is the OrdersV2Controller" });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrder(CreateOrderRequestV2Dto dto)
        {
            var result = await _orderServiceV2.CreateOrderAsync(dto);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Errors);
            }
            return Ok(result.Data);
        }

        [HttpPost("multi-error")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrderMultiError(CreateOrderRequestV2Dto dto)
        {
            var result = await _orderServiceV2.CreateOrderMultiErrorAsync(dto);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Errors);
            }
            return Ok(result.Data);
        }
    }
}
