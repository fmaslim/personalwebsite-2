using Microsoft.AspNetCore.Mvc;
using PersonalWebsite.Api.DTOs.Common;

namespace PersonalWebsite.Api.Extensions
{
    public static class ServiceResultExtensions
    {
        public static IActionResult ToActionResult<T>(this ServiceResult<T> result)
        {
            if (result.Success)
            {
                return new OkObjectResult(result.Data);
            }

            return result.StatusCode switch
            {
                400 => new BadRequestObjectResult(result),
                404 => new NotFoundObjectResult(result),
                409 => new ConflictObjectResult(result),
                _ => new ObjectResult(result)
                {
                    StatusCode = result.StatusCode,
                }
            };
        }
    }
}
