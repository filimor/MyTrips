using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyTrips.Presentation.Filters;

public class MediaTypeOperationFilter : IOperationFilter
{
    // TODO: Update it with the pagination schema
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var response in operation.Responses)
        {
            response.Value.Content.Clear();

            switch (response.Key)
            {
                case "200":
                    Set200OkResponseSchema(response);
                    break;
                case "201":
                    Set201CreatedResponseSchema(response);
                    break;
                case "400":
                    Set400BadRequestResponseSchema(response);
                    break;
                case "401":
                    continue;
                case "404":
                    Set404BadRequestResponseSchema(response);
                    break;

                case "409":
                    Set409ConflictResponseSchema(response);
                    break;

                case "429":
                    continue;
                case "500":
                    Set500InternalServerErrorResponseSchema(response);

                    break;
                default:
                    {
                        if (response.Key.StartsWith('4') || (response.Key.StartsWith('5') && response.Key != "500"))
                            response.Value.Content.Add("application/problem+json", new OpenApiMediaType());
                        else
                            response.Value.Content.Add("application/json", new OpenApiMediaType());
                        break;
                    }
            }
        }
    }

    private static void Set500InternalServerErrorResponseSchema(KeyValuePair<string, OpenApiResponse> response)
    {
        response.Value.Content.Add("application/problem+json", new OpenApiMediaType
        {
            Schema = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["type"] = new() { Type = "string", Example = new OpenApiString("about:blank") },
                    ["title"] = new()
                    { Type = "string", Example = new OpenApiString("Internal Server Error") },
                    ["status"] = new() { Type = "integer", Example = new OpenApiInteger(500) },
                    ["detail"] = new()
                    {
                        Type = "string",
                        Example = new OpenApiString("An unexpected error occurred. Please try again later.")
                    },
                    ["errors"] = new()
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "string",
                            Example = new OpenApiString("An unexpected error occurred.")
                        }
                    },
                    ["instance"] = new()
                    { Type = "string", Example = new OpenApiString("/api/clients/12345") }
                }
            }
        });
    }

    private static void Set409ConflictResponseSchema(KeyValuePair<string, OpenApiResponse> response)
    {
        response.Value.Content.Add("application/problem+json", new OpenApiMediaType
        {
            Schema = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["type"] = new() { Type = "string", Example = new OpenApiString("conflict") },
                    ["title"] = new() { Type = "string", Example = new OpenApiString("Conflict") },
                    ["status"] = new() { Type = "integer", Example = new OpenApiInteger(409) },
                    ["detail"] = new()
                    {
                        Type = "string",
                        Example = new OpenApiString(
                            "The request could not be completed due to a conflict with the current state of the resource.")
                    },
                    ["errors"] = new()
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "string",
                            Example = new OpenApiString("Duplicate entry")
                        }
                    },
                    ["instance"] = new()
                    { Type = "string", Example = new OpenApiString("/api/clients/12345") }
                }
            }
        });
    }

    private static void Set404BadRequestResponseSchema(KeyValuePair<string, OpenApiResponse> response)
    {
        response.Value.Content.Add("application/problem+json", new OpenApiMediaType
        {
            Schema = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["type"] = new()
                    {
                        Type = "string",
                        Example = new OpenApiString("https://example.com/probs/resource-not-found")
                    },
                    ["title"] =
                        new() { Type = "string", Example = new OpenApiString("Resource Not Found") },
                    ["status"] = new() { Type = "integer", Example = new OpenApiInteger(404) },
                    ["detail"] = new()
                    {
                        Type = "string",
                        Example = new OpenApiString("The requested resource was not found.")
                    },
                    ["errors"] = new()
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        { Type = "string", Example = new OpenApiString("No such client found") }
                    },
                    ["instance"] = new()
                    { Type = "string", Example = new OpenApiString("/api/clients/12345") }
                }
            }
        });
    }

    private static void Set400BadRequestResponseSchema(KeyValuePair<string, OpenApiResponse> response)
    {
        response.Value.Content.Add("application/problem+json", new OpenApiMediaType
        {
            Schema = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["type"] = new()
                    {
                        Type = "string",
                        Example = new OpenApiString("https://example.com/probs/invalid-request")
                    },
                    ["title"] = new() { Type = "string", Example = new OpenApiString("Invalid request.") },
                    ["status"] = new() { Type = "integer", Example = new OpenApiInteger(400) },
                    ["detail"] = new()
                    {
                        Type = "string",
                        Example = new OpenApiString("The request contains invalid parameters.")
                    },
                    ["errors"] = new()
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        { Type = "string", Example = new OpenApiString("Invalid client data") }
                    },
                    ["instance"] = new() { Type = "string", Example = new OpenApiString("/clients/12345") }
                }
            }
        });
    }

    private static void Set201CreatedResponseSchema(KeyValuePair<string, OpenApiResponse> response)
    {
        response.Value.Content.Add("application/json", new OpenApiMediaType
        {
            Schema = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["id"] = new() { Type = "integer", Example = new OpenApiInteger(123) },
                    ["name"] = new() { Type = "string", Example = new OpenApiString("John Doe") },
                    ["email"] = new()
                    { Type = "string", Example = new OpenApiString("john.doe@example.com") }
                }
            }
        });
    }

    private static void Set200OkResponseSchema(KeyValuePair<string, OpenApiResponse> response)
    {
        response.Value.Content.Add("application/json", new OpenApiMediaType
        {
            Schema = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["id"] = new() { Type = "integer", Example = new OpenApiInteger(123) },
                    ["name"] = new() { Type = "string", Example = new OpenApiString("John Doe") },
                    ["email"] = new()
                    { Type = "string", Example = new OpenApiString("john.doe@example.com") }
                }
            }
        });
    }
}