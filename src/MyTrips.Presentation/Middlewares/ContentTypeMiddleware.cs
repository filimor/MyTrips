namespace MyTrips.Presentation.Middlewares;

public class ContentTypeMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            if (context.Response.StatusCode >= 400)
                context.Response.ContentType = "application/problem+json; charset=utf-8";
            return Task.CompletedTask;
        });

        await next(context);
    }
}