using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Patients;
using PersonalWebsite.Api.Services.PerformanceTraining.Patients;

namespace PersonalWebsite.Api.Controllers.PerformanceTraining
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PatientSearchTrainingController : ControllerBase
    {
        private readonly IPatientSearchTrainingService _service;
        public PatientSearchTrainingController(IPatientSearchTrainingService service)
        {
            _service = service;
        }

        [HttpGet("search-patients")]
        public async Task<IActionResult> GetPatientsAsync([FromQuery] PatientSearchRequestDto requestDto)
        {
            var result = await _service.SearchPatientsAsync(requestDto);
            return Ok(result);
        }
    }
}
