using Microsoft.AspNetCore.Diagnostics;
using PersonalWebsite.Api.DTOs.Common;
using Microsoft.ApplicationInsights;

namespace PersonalWebsite.Api.ExceptionHandling
{
    public class GlobalExceptionHandling : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandling> _logger;
        private readonly TelemetryClient _telemetryClient;
        public GlobalExceptionHandling(ILogger<GlobalExceptionHandling> logger, TelemetryClient tclient)
        {
            _logger = logger;
            _telemetryClient = tclient;
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
            // Added AppInsight track exception manually
            _telemetryClient.TrackException(exception, new Dictionary<string, string>
            {
                ["Method"] = httpContext.Request.Method,
                ["Path"] = httpContext.Request.Path,
                ["QueryString"] = httpContext.Request.QueryString.ToString(),
                ["TraceId"] = httpContext.TraceIdentifier,
                ["ExceptionType"] = exception.GetType().Name
            });
            // Since it's not showing up, try flushing and adding a delay
            _telemetryClient.Flush();
            await Task.Delay(1000, cancellationToken);

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
