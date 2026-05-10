using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.Models.Errors;

namespace PersonalWebsite.Api.Extensions
{
    public static class ServiceResultExtensions
    {
        public static IActionResult ToActionResult<T>(this ServiceResult<T> result)
        {
            if (result.Success)
            {
                // return new OkObjectResult(result.Data);
                return new ObjectResult(result.Data)
                {
                    StatusCode = result.StatusCode,
                };
            }

            return result.ServiceErrorType switch
            {
                ServiceErrorType.Validation => new BadRequestObjectResult(result),
                ServiceErrorType.NotFound => new NotFoundObjectResult(result),
                ServiceErrorType.Conflict => new ConflictObjectResult(result),
                ServiceErrorType.Unexpected => new ObjectResult(result)
                { 
                    StatusCode = 500
                },
                _ => new ObjectResult(result) 
                {
                    StatusCode = result.StatusCode,
                }
            };
        }
    }
}
