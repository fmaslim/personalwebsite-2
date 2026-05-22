using PersonalWebsite.Api.DTOs.Auth;
using PersonalWebsite.Api.DTOs.Common;
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

        public async Task<ServiceResult<LoginResponseV3Dto>> LoginV3Async(LoginRequestV3Dto dto)
        {
            if (dto == null)
            {
                return ServiceResult<LoginResponseV3Dto>.Fail("Request body is required.", 400);
            }

            // if missing username or password, return 400 Bad Request
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            {                 
                return ServiceResult<LoginResponseV3Dto>.Fail("Username and password are required.", 400);
            }

            // if invalid username or password, return 401 Unauthorized
            if (dto.Username != "franky" || dto.Password != "123")
            {
                return ServiceResult<LoginResponseV3Dto>.Fail("Invalid username or password.", 401);
            }

            // if valid, return 200 OK with user info and token
            var response = new LoginResponseV3Dto
            {
                Username = dto.Username,
                Message = "Login successful from v3",
                Version = "v3",
                ExpiresIn = 3600,
                Token = "fake-jwt-token-v3",
                TokenType = "Bearer",
                RefreshToken = "fake-refresh-token-v3"
            };
            
            return ServiceResult<LoginResponseV3Dto>.Ok(response, "Login successful");
        }
    }
}
