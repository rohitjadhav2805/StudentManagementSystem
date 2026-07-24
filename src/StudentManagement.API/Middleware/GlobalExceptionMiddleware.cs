using System.Net;
using System.Text.Json;
using StudentManagement.Application.Exceptions;
using StudentManagement.Domain.Common;

namespace StudentManagement.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized && !context.Response.HasStarted)
            {
                await HandleCustomResponseAsync(context, HttpStatusCode.Unauthorized, "Unauthorized access. Valid JWT token required.", new List<string> { "Authentication failed." });
            }
            else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden && !context.Response.HasStarted)
            {
                await HandleCustomResponseAsync(context, HttpStatusCode.Forbidden, "Access forbidden. Insufficient permissions.", new List<string> { "Forbidden resource." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            BadRequestException badRequestEx => (HttpStatusCode.BadRequest, badRequestEx.Message, badRequestEx.Errors),
            DuplicateEmailException duplicateEx => (HttpStatusCode.Conflict, duplicateEx.Message, new List<string> { duplicateEx.Message }),
            NotFoundException notFoundEx => (HttpStatusCode.NotFound, notFoundEx.Message, new List<string> { notFoundEx.Message }),
            UnauthorizedException unauthorizedEx => (HttpStatusCode.Unauthorized, unauthorizedEx.Message, new List<string> { unauthorizedEx.Message }),
            KeyNotFoundException keyNotFoundEx => (HttpStatusCode.NotFound, keyNotFoundEx.Message, new List<string> { keyNotFoundEx.Message }),
            UnauthorizedAccessException authEx => (HttpStatusCode.Forbidden, "Access forbidden.", new List<string> { authEx.Message }),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.", new List<string> { exception.Message })
        };

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.FailureResponse(message, (int)statusCode, errors);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }

    private static async Task HandleCustomResponseAsync(HttpContext context, HttpStatusCode statusCode, string message, List<string> errors)
    {
        context.Response.ContentType = "application/json";
        var response = ApiResponse<object>.FailureResponse(message, (int)statusCode, errors);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
