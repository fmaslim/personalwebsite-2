using PersonalWebsite.Api.DTOs.Auth;
using PersonalWebsite.Api.DTOs.Common;

namespace PersonalWebsite.Api.Services.Abstractions
{
    public interface IAuthService
    {
        Task<LoginV2ResultDto> LoginV2Async(LoginRequestDto dto);

        Task<ServiceResult<LoginResponseV3Dto>> LoginV3Async(LoginRequestV3Dto dto);
    }
}
