using Microsoft.OpenApi.Models;
using MyTrips.Application.Dtos;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyTrips.Presentation.Filters;

public class SwaggerSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(CreateClientDto)) schema.Title = "Client";
    }
}