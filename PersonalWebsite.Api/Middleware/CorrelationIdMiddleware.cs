namespace PersonalWebsite.Api.Middleware;

public class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(
        RequestDelegate next,
        ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var existingId)
            ? existingId.ToString()
            : Guid.NewGuid().ToString();

        //context.Response.Headers[HeaderName] = correlationId;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });
        context.Items["CorrelationId"] = correlationId;

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            _logger.LogInformation(
            "Request started. CorrelationId={CorrelationId}. Method={Method}. Path={Path}",
            correlationId,
            context.Request.Method,
            context.Request.Path);

            await _next(context);

            _logger.LogInformation(
            "Request completed. CorrelationId={CorrelationId}. StatusCode={StatusCode}",
            correlationId,
            context.Response.StatusCode);
        }
    }
}