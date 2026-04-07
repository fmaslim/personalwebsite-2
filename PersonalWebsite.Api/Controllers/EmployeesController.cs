using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
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
        public async Task<ActionResult<IEnumerable<EmployeeLookupDto>>> GetEmployees()
        {
            var employees = await _productService.GetEmployeeListAsync();
            return Ok(employees);
        }

        [HttpGet("{employeeId}")]
        public async Task<ActionResult<EmployeeLookupDto>> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }
    }
}
