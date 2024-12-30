using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyTrips.Application.Validators;
using MyTrips.CrossCutting;
using MyTrips.Presentation.Errors;
using MyTrips.Presentation.Filters;
using Serilog;

try
{
    CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
    ValidatorOptions.Global.LanguageManager.Enabled = false;
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
        .WriteTo.Debug(formatProvider: CultureInfo.InvariantCulture)
        .CreateLogger();

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddProblemDetails(options =>
    {
        options.IncludeExceptionDetails = (context, _) =>
            context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment() ||
            context.RequestServices.GetRequiredService<IHostEnvironment>().IsStaging();
    });
    //builder.Services.AddExceptionHandler(options =>
    //{
    //    options.ExceptionHandler = 
    //});
    builder.Services.AddControllers(options => { options.Filters.Add(new ProblemHeaderFilter()); });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1",
            new OpenApiInfo
            {
                Title = "MyTrips API",
                Contact = new OpenApiContact
                {
                    Email = "filimor@posteo.net",
                    Name = "Filipe Moreira",
                    Url = new Uri("https://mytrips.azurewebsites.net/")
                },
                Description = "MyTrips is an web service to manage data about trips and clients.",
                Version = "v0.1",
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        opt.IncludeXmlComments(xmlPath);

        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Description = "Copy 'Bearer ' + token",
            Type = SecuritySchemeType.ApiKey
        });

        opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        opt.OperationFilter<MediaTypeOperationFilter>();

        opt.SchemaFilter<SwaggerSchemaFilter>();
    });

    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication("Bearer").AddJwtBearer(
        "JwtScheme", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:JwtSecret"]!))
            };
        });

    builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddValidatorsFromAssemblyContaining<ClientValidator>();

    builder.Services.AddHttpLogging(options => { options.LoggingFields = HttpLoggingFields.All; });
    builder.Services.AddSerilog();

    var defaultCulture = CultureInfo.InvariantCulture;
    var localizationOptions = new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture(defaultCulture),
        SupportedCultures = [defaultCulture],
        SupportedUICultures = [defaultCulture]
    };

    builder.Services.AddCors(options =>
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
        }));


    builder.Services.AddRateLimiter(options =>
    {
        if (builder.Configuration.GetValue<bool>("DisableRateLimiter")) return;

        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            return RateLimitPartition.GetFixedWindowLimiter("global", _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 30,
                QueueLimit = 10,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                Window = TimeSpan.FromSeconds(30)
            });
        });
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseProblemDetails();
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

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseRequestLocalization(localizationOptions);
    app.UseHttpsRedirection();
    app.UseCors();
    app.UseRateLimiter();
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