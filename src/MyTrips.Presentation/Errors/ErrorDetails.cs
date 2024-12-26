using System.Text.Json;
using System.Text.Json.Serialization;
using FluentResults;

namespace MyTrips.Presentation.Errors;

public class ErrorDetails
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public string Type { get; set; } = null!;
    public string Title { get; set; } = null!;
    public int Status { get; set; }
    public IEnumerable<IError> Detail { get; set; } = null!;
    public string Instance { get; set; } = "about:blank";

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, JsonOptions);
    }
}