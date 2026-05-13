using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.Patients;
using PersonalWebsite.Api.Extensions;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;
        public PatientsController(IPatientService service)
        {
            _patientService = service;
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchPatientsAsync(
            [FromQuery] string? firstName,
            [FromQuery] string? lastName,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortDir,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _patientService.SearchPatientsAsync(firstName, lastName, sortBy, sortDir, pageNumber, pageSize);
            return result.ToActionResult();
        }
    }
}
