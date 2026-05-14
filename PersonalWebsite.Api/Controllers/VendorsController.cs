using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Extensions;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/vendors")]
    public class VendorsController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        public VendorsController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }

        [HttpGet("v2/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VendorDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVendorByIdAsync(int id)
        {
            var result = await _vendorService.GetVendorByIdV2Async(id);
            return result.ToActionResult();
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<VendorDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<VendorDto>>> SearchVendorsByNameAsync(
            [FromQuery] string? name = null,
            [FromQuery] int page = 1,
            [FromQuery]  int pageSize = 10,
            [FromQuery] string? sortBy = "name",
            [FromQuery] string? sortDir = "asc")
        {
            var vendors = await _vendorService.SearchVendorsByNameAsync(name, page, pageSize, sortBy, sortDir);
            return Ok(vendors);
        }
    }
}
