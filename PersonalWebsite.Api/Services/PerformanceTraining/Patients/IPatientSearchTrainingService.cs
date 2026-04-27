using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Patients;

namespace PersonalWebsite.Api.Services.PerformanceTraining.Patients
{
    public interface IPatientSearchTrainingService
    {
        Task<PagedResponse<PatientSearchResultDto>> SearchPatientsAsync(PatientSearchRequestDto requestDto);
    }
}
