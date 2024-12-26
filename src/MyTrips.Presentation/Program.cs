using System.Globalization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Localization;
using MyTrips.CrossCutting;
using MyTrips.Presentation.Filters;
using MyTrips.Presentation.Middlewares;
using Serilog;

try
{
    CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
        .WriteTo.Debug(formatProvider: CultureInfo.InvariantCulture)
        .CreateLogger();

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers(options => { options.Filters.Add(new ProblemHeaderFilter()); });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
    builder.Services.AddTransient<ExceptionHandlingMiddleware>();
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddHttpLogging(options => { options.LoggingFields = HttpLoggingFields.All; });
    builder.Services.AddSerilog();

    var defaultCulture = CultureInfo.InvariantCulture;
    var localizationOptions = new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture(defaultCulture),
        SupportedCultures = [defaultCulture],
        SupportedUICultures = [defaultCulture]
    };

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseRequestLocalization(localizationOptions);
    app.UseHttpsRedirection();
    app.UseCors();
    app.UseAuthorization();
    app.MapControllers();

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


/// <summary>
///     Entrypoint
/// </summary>
public partial class Program
{
}