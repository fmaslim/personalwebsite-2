using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
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

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<VendorDto>>> GetAllVendorsAsync()
        //{
        //     var vendors = await _vendorService.GetAllVendorsAsync();
        //    return Ok(vendors);
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<VendorDto?>> GetVendorByIdAsync(int id)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if(vendor == null)
            {
                return NotFound();
            }
            return Ok(vendor);
        }

        [HttpGet("search")]
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
