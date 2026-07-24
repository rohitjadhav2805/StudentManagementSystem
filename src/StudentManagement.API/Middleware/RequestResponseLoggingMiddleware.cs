using System.Text;

namespace StudentManagement.API.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("HTTP Request: {Method} {Path}{QueryString}", 
            context.Request.Method, 
            context.Request.Path, 
            context.Request.QueryString);

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            _logger.LogInformation("HTTP Response: {Method} {Path} responded {StatusCode}", 
                context.Request.Method, 
                context.Request.Path, 
                context.Response.StatusCode);

            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
}
