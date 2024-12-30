using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using MyTrips.Application.Dtos;
using MyTrips.Domain.ValueObjects;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyTrips.Presentation.Filters;

public class SwaggerSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(CreateClientDto)) schema.Title = "Create Client Request";
        else if (context.Type == typeof(ProblemDetails)) schema.Title = "Error Response";
        else if (context.Type == typeof(ResponseClientDto)) schema.Title = "Client Response";
        else if (context.Type == typeof(UpdateClientDto)) schema.Title = "Update Client Request";
        else if (context.Type == typeof(LoginInfo)) schema.Title = "Login";
        else if (context.Type == typeof(CreateTripDto)) schema.Title = "Create Trip Request";
        else if (context.Type == typeof(ResponseDestinationDto)) schema.Title = "Destination";
        else if (context.Type == typeof(ResponseFlightDto)) schema.Title = "Flight";
        else if (context.Type == typeof(ResponseHotelDto)) schema.Title = "Hotel";
        else if (context.Type == typeof(ResponseTripDto)) schema.Title = "Trip Response";
    }
}