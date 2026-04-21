using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class AuthService : IAuthService
    {
        public Task<LoginV2ResultDto> LoginV2Async(LoginRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            {
                // return BadRequest(new LoginErrorResponseV2Dto { Message = "Username and password are required." });
                return Task.FromResult(new LoginV2ResultDto
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Username and password are required.",
                    Data = null
                });
            }

            if (dto.Username != "franky" || dto.Password != "123")
            {
                return Task.FromResult(new LoginV2ResultDto
                {
                    Success = false,
                    StatusCode = 401,
                    Message = "Invalid username or password.",
                    Data = null
                });
            }

            var response = new LoginResponseV2Dto
            {
                Username = dto.Username,
                Message = "Login successful from v2",
                Version = "v2",
                ExpiresIn = 3600,
                Token = "fake-jwt-token-v2",
                RefreshToken = "fake-refresh-token-v2"
            };

            return Task.FromResult(new LoginV2ResultDto
            {
                Success = true,
                StatusCode = 200,
                Message = "Login successful",
                Data = response
            });
        }
    }
}
