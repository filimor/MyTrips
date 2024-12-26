using MyTrips.Presentation.Errors;

namespace MyTrips.Presentation.Middlewares;

public class ExceptionHandlingMiddleware(IHostEnvironment environment)
    : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var errorDetails = new InternalServerErrorDetails(context);

        if (environment.IsDevelopment())
            errorDetails.Errors =
            [
                exception.GetType().Name,
                exception.Message,
                exception.StackTrace ?? string.Empty,
                exception.InnerException?.ToString() ?? string.Empty,
                exception.Data.ToString() ?? string.Empty,
                exception.TargetSite?.ToString() ?? string.Empty
            ];

        context.Response.StatusCode = errorDetails.Status;
        context.Response.ContentType = "application/problem+json; charset=utf-8";

        await context.Response.WriteAsync(errorDetails.ToString());
    }
}