using PersonalWebsite.Api.DTOs.Common;
// using PersonalWebsite.Api.DTOs.Patients;
using PersonalWebsite.Api.DTOs.PerformanceTraining.Patients;
using PersonalWebsite.Api.Extensions;

namespace PersonalWebsite.Api.Services.PerformanceTraining.Patients
{
    public class PatientSearchTrainingService : IPatientSearchTrainingService
    {
        public Task<PagedResponse<PatientSearchResultDto>> SearchPatientsAsync(PatientSearchRequestDto requestDto)
        {
            var patients = new List<PatientSearchResultDto>();

            var patientA = new PatientSearchResultDto();
            patientA.PatientId = 1;
            patientA.FullName = "John Smith";
            patientA.Age = 42;
            patientA.Gender = "Male";
            patientA.Status = "Active";
            patientA.LastVisitDate = DateTime.UtcNow.AddDays(-2);
            patients.Add(patientA);

            var patientB = new PatientSearchResultDto();
            patientB.PatientId = 2;
            patientB.FullName = "Maria Garcia";
            patientB.Age = 35;
            patientB.Gender = "Female";
            patientB.Status = "Active";
            patientB.LastVisitDate = DateTime.UtcNow.AddDays(-10);
            patients.Add(patientB);

            var patientC = new PatientSearchResultDto();
            patientC.PatientId = 3;
            patientC.FullName = "David Lee";
            patientC.Age = 67;
            patientC.Gender = "Male";
            patientC.Status = "Inactive";
            patientC.LastVisitDate = DateTime.UtcNow.AddDays(-45);
            patients.Add(patientC);

            var finalResult = patients.ToPagedResponse(requestDto.PageNumber, requestDto.PageSize);

            return Task.FromResult(finalResult);
        }
    }
}
