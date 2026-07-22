using System.Net;
using System.Text.Json;
using FluentValidation;
using RentalSystem.Domain.Exceptions;

namespace RentalSystem.API.Middleware;

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
 
            EmailAlreadyExistsException => (HttpStatusCode.Conflict, exception.Message),
            InvalidCredentialsException => (HttpStatusCode.Unauthorized, exception.Message),
            AccountDeactivatedException => (HttpStatusCode.Forbidden, exception.Message), 
            InvalidRefreshTokenException => (HttpStatusCode.Unauthorized, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        // Log the FULL exception internally, regardless of what we tell the client
        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception occurred");
        else
            _logger.LogWarning("Handled exception: {Message}", exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            status = (int)statusCode,
            message,
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

}