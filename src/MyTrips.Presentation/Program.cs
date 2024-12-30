using System.Globalization;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Localization;
using MyTrips.CrossCutting;
using MyTrips.Presentation.Extensions;
using MyTrips.Presentation.Middlewares;
using Serilog;

try
{
    CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
    SetupSerilog();

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddLoggingServices();
    builder.Services.AddSingleton(builder.Configuration);
    builder.Services.AddConfigurations(builder.Configuration);
    builder.Services.AddValidation();
    builder.Services.AddProblemDetailsOptions();
    builder.Services.AddControllers();
    builder.Services.AddSwagger();
    builder.Services.AddDomainServices();
    builder.Services.AddAuthServices(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApiSecurity(builder.Configuration);

    var app = builder.Build();


    app.UseSerilogRequestLogging();
    app.UseProblemDetails();
    app.UseExceptionHandlerMiddleware();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
    }


    app.UseRequestLocalization(SetupLocalization());
    app.UseHttpsRedirection();
    app.UseCors();
    app.UseRateLimiter();
    app.UseAuthorization();
    app.MapControllers();
    app.UseMiddleware<ContentTypeMiddleware>();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}

return;

RequestLocalizationOptions SetupLocalization()
{
    var defaultCulture = CultureInfo.InvariantCulture;
    var requestLocalizationOptions = new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture(defaultCulture),
        SupportedCultures = [defaultCulture],
        SupportedUICultures = [defaultCulture]
    };
    return requestLocalizationOptions;
}

void SetupSerilog()
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
        .WriteTo.Debug(formatProvider: CultureInfo.InvariantCulture)
        .CreateLogger();
}


/// <summary>
///     Entrypoint
/// </summary>
public partial class Program
{
}