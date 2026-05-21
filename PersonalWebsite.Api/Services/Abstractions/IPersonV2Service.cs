using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;

namespace PersonalWebsite.Api.Services.Abstractions;

public interface IPersonV2Service
{
    Task<ServiceResult<PagedResponseDto<PersonSearchDto>>> SearchPersonsAsync(
        string name,
        string sortBy,
        string sortDir,
        int pageNumber,
        int pageSize);
}
