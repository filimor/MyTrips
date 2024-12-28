using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class MediaTypeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var response in operation.Responses)
        {
            response.Value.Content.Clear();

            if (response.Key.StartsWith('4') || response.Key.StartsWith('5'))
                response.Value.Content.Add("application/problem+json", new OpenApiMediaType());
            else
                response.Value.Content.Add("application/json", new OpenApiMediaType());
        }
    }
}