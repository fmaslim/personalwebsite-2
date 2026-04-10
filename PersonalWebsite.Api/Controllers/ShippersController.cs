using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/shippers")]
    public class ShippersController : ControllerBase
    {
        private readonly IShipperService _shipperService;
        public ShippersController(IShipperService shipperService)
        {
            _shipperService = shipperService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipperDto>>> GetAllShippersAsync()
        {
            var shippers = await _shipperService.GetAllShippersAsync();
            return Ok(shippers);
        }

        [HttpGet("{shipperId}")]
        public async Task<ActionResult<ShipperDto?>> GetShipperByIdAsync(int shipperId)
        {
            var shipper = await _shipperService.GetShipperByIdAsync(shipperId);
            if (shipper == null)
            {
                return NotFound();
            }
            return Ok(shipper);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ShipperDto>>> SearchShippersAsync(
            [FromQuery] string? name = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "shipmethodid",
            [FromQuery] string? sortDir = "asc")
        {
            var shippers = await _shipperService.SearchShippersAsync(name, page, pageSize, sortBy, sortDir);
            return Ok(shippers);
        }
    }
}
