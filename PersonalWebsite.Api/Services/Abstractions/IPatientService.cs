using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IPatientService
    {
        Task<PagedPatientSearchResponseDto> SearchPatientsAsync(
            string? firstName,
            string? lastName,
            string? sortBy,
            string? sortDir,
            int pageNumber,
            int pageSize);
    }
}
