using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyTrips.Application.Interfaces;
using MyTrips.Application.Services;
using MyTrips.Application.Validators;
using MyTrips.Infrastructure.Models;
using MyTrips.Presentation.Filters;
using Serilog;

namespace MyTrips.Presentation.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(opt =>
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

        return services;
    }

    public static IServiceCollection AddApiSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            }));


        services.AddRateLimiter(options =>
        {
            if (configuration.GetValue<bool>("DisableRateLimiter")) return;

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

        return services;
    }

    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthService, AuthService>();

        services.AddAuthorization();

        services.AddAuthentication("Bearer").AddJwtBearer(
            "JwtScheme", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:JwtSecret"]!))
                };
            });

        return services;
    }

    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IClientsService, ClientsService>();

        return services;
    }

    public static IServiceCollection AddLoggingServices(this IServiceCollection services)
    {
        services.AddHttpLogging(options => { options.LoggingFields = HttpLoggingFields.All; });

        services.AddSerilog();

        return services;
    }

    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        ValidatorOptions.Global.LanguageManager.Enabled = false;

        services.AddValidatorsFromAssemblyContaining<ClientValidator>();

        return services;
    }

    public static IServiceCollection AddProblemDetailsOptions(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.IncludeExceptionDetails = (context, _) =>
                context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment() ||
                context.RequestServices.GetRequiredService<IHostEnvironment>().IsStaging();
        });

        return services;
    }

    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);

        services.Configure<AppSetting>(configuration.GetSection(nameof(AppSetting)));

        return services;
    }
}