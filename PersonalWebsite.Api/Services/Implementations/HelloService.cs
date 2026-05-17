using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Services.Abstractions;

namespace PersonalWebsite.Api.Services.Implementations
{
    public class HelloService : IHelloService
    {
        private readonly ILogger<HelloService> _logger;

        public HelloService(ILogger<HelloService> logger)
        {
            _logger = logger;
        }

        public async Task<ServiceResult<HelloResponseDto>> GetGreetingAsync(string name)
        {
            // ensure this method is asynchronous for API consistency
            await Task.Yield();

            if (string.IsNullOrWhiteSpace(name))
            {
                return ServiceResult<HelloResponseDto>.Fail(
                    "Name is required.",
                    Models.Errors.ServiceErrorType.Validation);
            }

            if (name.Length > 50)
            {
                return ServiceResult<HelloResponseDto>.Fail(
                    "Name cannot be longer than 50 characters.",
                    Models.Errors.ServiceErrorType.Validation);
            }

            var dto = new HelloResponseDto
            {
                Message = $"Hello, {name}!"
            };

            return ServiceResult<HelloResponseDto>.Ok(dto);
        }
    }
}
