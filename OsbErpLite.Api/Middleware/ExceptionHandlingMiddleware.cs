using System.Net;
using System.Text.Json;
using FluentValidation;
using OsbErpLite.Api.Common;

namespace OsbErpLite.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                string.Join(" | ", validationEx.Errors.Select(e => e.ErrorMessage))
            ),
            KeyNotFoundException => (
                HttpStatusCode.NotFound,
                exception.Message
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred."
            )
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception");
        else
            _logger.LogWarning(exception, "Handled exception: {StatusCode}", (int)statusCode);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Fail(message);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
