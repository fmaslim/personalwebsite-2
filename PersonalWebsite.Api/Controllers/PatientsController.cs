using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs;
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
        public async Task<ActionResult<IEnumerable<PatientSearchResultDto>>> SearchPatientsAsync(
            [FromQuery] string? firstName,
            [FromQuery] string? lastName,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _patientService.SearchPatientsAsync(firstName, lastName, pageNumber, pageSize);
            return Ok(result);
        }
    }
}
