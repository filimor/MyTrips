using System.Text.Json.Serialization;
using FluentResults;

namespace MyTrips.Presentation.Errors;

public class CustomError : IError
{
    public string Message { get; set; } = null!;

    [JsonIgnore] public Dictionary<string, object> Metadata { get; set; } = null!;

    [JsonIgnore] public List<IError> Reasons { get; set; } = null!;
}