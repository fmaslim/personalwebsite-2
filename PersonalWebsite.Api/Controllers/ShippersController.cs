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
    }
}
