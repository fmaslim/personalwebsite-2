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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendorDto>>> GetAllVendorsAsync()
        {
             var vendors = await _vendorService.GetAllVendorsAsync();
            return Ok(vendors);
        }

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
    }
}
