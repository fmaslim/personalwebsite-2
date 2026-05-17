using PersonalWebsite.Api.DTOs;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.Models;
using PersonalWebsite.Api.Models.Errors;
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
                var fieldErrors = new List<FieldError>
                {
                    new FieldError { Field = "name", Message = "Name is required." }
                };

                return ServiceResult<HelloResponseDto>.ValidationFail(fieldErrors);
            }

            if (name.Length > 50)
            {
                var fieldErrors = new List<FieldError>
                {
                    new FieldError { Field = "name", Message = "Name cannot be longer than 50 characters." }
                };

                return ServiceResult<HelloResponseDto>.ValidationFail(fieldErrors);
            }

            var dto = new HelloResponseDto
            {
                Message = $"Hello, {name}!"
            };

            return ServiceResult<HelloResponseDto>.Ok(dto);
        }
    }
}
