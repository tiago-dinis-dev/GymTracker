using Application.Exceptions;

namespace Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

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
        catch(AppException ex)
        {
            _logger.LogWarning(ex, "A handled application exception occurred.");
            await HandleAppExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleUnexpectedExceptionAsync(context, ex);
        }
    }

    private static Task HandleAppExceptionAsync(HttpContext context, AppException ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = ex.StatusCode;

        var response = new
        {
            error = ex.Message
        };
        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleUnexpectedExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new
        {
            error = "An unexpected error occurred. Please try again later."
        };
        return context.Response.WriteAsJsonAsync(response);
    }
}
