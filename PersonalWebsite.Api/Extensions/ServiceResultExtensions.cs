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
                if (result.StatusCode == StatusCodes.Status204NoContent)
                {
                    return new NoContentResult();
                }
                
                return new ObjectResult(result.Data)
                {
                    StatusCode = result.StatusCode,
                };
            }

            var errorResponse = new DTOs.Common.ApiErrorResponse
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors,
                FieldErrors = result.FieldErrors,
                StatusCode = result.StatusCode
            };

            return result.ServiceErrorType switch
            {
                ServiceErrorType.Validation => new BadRequestObjectResult(errorResponse),
                ServiceErrorType.NotFound => new NotFoundObjectResult(errorResponse),
                ServiceErrorType.Conflict => new ConflictObjectResult(errorResponse),
                ServiceErrorType.Unexpected => new ObjectResult(errorResponse)
                { 
                    StatusCode = 500
                },
                _ => new ObjectResult(errorResponse) 
                {
                    StatusCode = result.StatusCode,
                }
            };
        }
    }
}
