using PersonalWebsite.Api.DTOs;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IAuthService
    {
        Task<LoginV2ResultDto> LoginV2Async(LoginRequestDto dto);
    }
}
