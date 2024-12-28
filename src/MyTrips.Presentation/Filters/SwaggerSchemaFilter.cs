using Microsoft.OpenApi.Models;
using MyTrips.Application.Dtos;
using MyTrips.Presentation.Errors;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyTrips.Presentation.Filters;

public class SwaggerSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(CreateClientDto)) schema.Title = "Create Client Request";
        else if (context.Type == typeof(ErrorDetails)) schema.Title = "Error Response";
        else if (context.Type == typeof(ResponseClientDto)) schema.Title = "Client Response";
        else if (context.Type == typeof(UpdateClientDto)) schema.Title = "Update Client Request";
    }
}