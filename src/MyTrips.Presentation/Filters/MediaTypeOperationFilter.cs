using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyTrips.Presentation.Filters;

public class MediaTypeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        string[] problemDetailsKeys = ["400", "404", "409", "500"];
        string[] noContentKeys = ["204", "401", "429"];

        foreach (var response in operation.Responses)
            if (noContentKeys.Contains(response.Key))
            {
                response.Value.Content.Clear();
                response.Value.Content.Add("application/problem+json", new OpenApiMediaType());
            }
            else if (problemDetailsKeys.Contains(response.Key))
            {
                response.Value.Content.Clear();

                response.Value.Content.Add("application/problem+json", new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["type"] = new() { Type = "string", Example = new OpenApiString("string") },
                            ["title"] = new() { Type = "string", Example = new OpenApiString("string") },
                            ["status"] = new() { Type = "integer", Example = new OpenApiInteger(500) },
                            ["detail"] = new()
                            {
                                Type = "string",
                                Example = new OpenApiString("string")
                            },
                            ["errors"] = new()
                            {
                                Type = "array",
                                Items = new OpenApiSchema
                                {
                                    Type = "string",
                                    Example = new OpenApiString("string")
                                }
                            },
                            ["instance"] = new()
                                { Type = "string", Example = new OpenApiString("string") }
                        }
                    }
                });
            }
    }
}