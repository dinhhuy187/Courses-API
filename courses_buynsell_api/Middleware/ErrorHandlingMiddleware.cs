using System.Net;
using System.Text.Json;
using courses_buynsell_api.Exceptions;

namespace courses_buynsell_api.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
            // Nếu status là 401 hoặc 403 mà chưa có nội dung trả về -> thêm JSON message
            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                await WriteErrorResponse(context, HttpStatusCode.Unauthorized, "Unauthorized: You are not authorized to access this resource.");
            }
            else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                await WriteErrorResponse(context, HttpStatusCode.Forbidden, "Forbidden: You do not have permission to access this resource.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            BadRequestException => HttpStatusCode.BadRequest,
            UnauthorizedException => HttpStatusCode.Unauthorized,
            NotFoundException => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };

        var response = new
        {
            message = exception.Message,
            detail = exception.InnerException?.Message
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode status, string message)
    {
        if (context.Response.HasStarted) return;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;
        var response = new { message };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}