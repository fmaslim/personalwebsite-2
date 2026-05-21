using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.Extensions;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IEmployeeService _employeeService;
        public EmployeesController(IProductService productService, IEmployeeService employeeService)
        {
            _productService = productService;
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _productService.GetEmployeeListV2Async();
            return employees.ToActionResult();  
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployeeByIdAsync(int employeeId)
        {
            var result = await _employeeService.GetEmployeeByIdAsync(employeeId);
            return result.ToActionResult();
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResponse<EmployeeLookupDto>>> SearchEmployeesAsync(
            [FromQuery] string? name,
            [FromQuery] string? jobTitle = null,
            [FromQuery] bool? currentFlag = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "employeeId",
            [FromQuery] string? sortDir = "asc")
        {
            var employees = await _employeeService.SearchEmployeesAsync(name, jobTitle, currentFlag, page, pageSize, sortBy, sortDir);
            return Ok(employees);
        }
    }
}
