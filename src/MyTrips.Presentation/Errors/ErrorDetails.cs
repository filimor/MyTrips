using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MyTrips.Presentation.Errors;

public class ErrorDetails
{
    protected static readonly JsonSerializerSettings JsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore
    };

    public string? Type { get; set; }
    public string? Title { get; set; }
    public int Status { get; set; }
    public string? Detail { get; set; }
    public string[]? Errors { get; set; }

    public string Instance { get; set; } = "about:blank";

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, JsonSettings);
    }
}