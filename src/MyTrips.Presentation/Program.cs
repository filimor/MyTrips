using Microsoft.AspNetCore.HttpLogging;
using MyTrips.CrossCutting;
using Serilog;

try
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        //.MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
        //.MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
        //.MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
        .WriteTo.Console()
        .WriteTo.Debug()
        .CreateLogger();

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddHttpLogging(options => { options.LoggingFields = HttpLoggingFields.All; });
    builder.Services.AddSerilog();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();
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