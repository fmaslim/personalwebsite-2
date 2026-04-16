using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientSearchResultDto>> SearchPatientsAsync(
            string? firstName,
            string? lastName,
            // string? city,
            // bool? isActive,            
            string? sortBy,
            string? sortDir,
            int pageNumber,
            int pageSize);
    }
}
