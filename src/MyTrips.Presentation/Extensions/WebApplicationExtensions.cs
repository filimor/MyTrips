using Microsoft.AspNetCore.Diagnostics;
using MyTrips.Presentation.Errors;
using Serilog;

namespace MyTrips.Presentation.Extensions;

public static class WebApplicationExtensions
{
    public static void UseExceptionHandlerMiddleware(this WebApplication app)
    {
        app.UseExceptionHandler(options =>
        {
            options.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (contextFeature != null) Log.Fatal(contextFeature.Error, "Unexpected error.");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/problem+json; charset=utf-8";
                await context.Response.WriteAsJsonAsync(new InternalServerErrorProblemDetails(context));
            });
        });
    }
}