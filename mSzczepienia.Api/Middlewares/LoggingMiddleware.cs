using System.Diagnostics;

namespace mSzczepienia.Api.Middlewares;

public sealed class LoggingMiddleware(ILogger<LoggingMiddleware> logger) : IMiddleware
{
    private const string ReceivedRequestLogMessage = "Request: {Method} {Path}";
    private const string ResponseLogMessage = "Response: {StatusCode} {Method} {Path} - {ElapsedMilliseconds}ms";
    private const string ErrorLogMessage = "An unhandled exception occurred while processing the request.";
    private const string InternalServerErrorMessage = "An internal server error occurred. Please try again later.";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            logger.LogInformation(ReceivedRequestLogMessage,
                context.Request.Method,
                context.Request.Path);

            await next(context);

            logger.LogInformation(ResponseLogMessage,
                context.Response.StatusCode,
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    private async Task HandleException(HttpContext context, Exception ex)
    {
        logger.LogError(ex, ErrorLogMessage);

        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                Message = InternalServerErrorMessage
            };

            await context.Response.WriteAsJsonAsync(response);
        }
        else
        {
            logger.LogWarning("The response has already started, unable to send custom error message.");
        }
    }
}