using Microsoft.AspNetCore.Diagnostics;
using PersonalWebsite.Api.DTOs.Common;

namespace PersonalWebsite.Api.ExceptionHandling
{
    public class GlobalExceptionHandling : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandling> _logger;
        public GlobalExceptionHandling(ILogger<GlobalExceptionHandling> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // _logger.LogError(exception, "An unhandled exception occurred");
            _logger.LogError(
                exception,
                "Unhandled exception occurred. Method: {Method}, Path {Path}, QueryString: {QueryString}, TraceId: {TraceId}, Exception Type: {ExceptionType}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.Request.QueryString,
                httpContext.TraceIdentifier,
                exception.GetType().Name
                );

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var result = ServiceResult<string>.Fail(
            code: "UnexpectedError",
            message: "Something went wrong. Please try again later.",
            statusCode: 500);

            await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);

            return true;
        }
    }
}
