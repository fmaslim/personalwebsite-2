using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.Common;
using PersonalWebsite.Api.Models.Errors;

namespace PersonalWebsite.Api.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        protected IActionResult ToActionResult<T>(ServiceResult<T> result)
        {
            if (result.Success)
            {
                return Ok(result);
            }

            var errorType = result.Errors.FirstOrDefault()?.Type;

            return errorType switch
            {
                ServiceErrorType.Validation => BadRequest(result),
                ServiceErrorType.NotFound => NotFound(result),
                ServiceErrorType.Conflict => Conflict(result),
                _ => StatusCode(result.StatusCode, result)
            };
        }
    }
}
