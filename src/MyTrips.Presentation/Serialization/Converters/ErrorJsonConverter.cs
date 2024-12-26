using System.Text.Json;
using System.Text.Json.Serialization;
using FluentResults;
using MyTrips.Presentation.Errors;

namespace MyTrips.Presentation.Serialization.Converters;

// TODO: Move it to the test class
public class ErrorJsonConverter : JsonConverter<IError>
{
    public override IError Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;

        var customError = new CustomError
        {
            Message = jsonObject.GetProperty(nameof(CustomError.Message)).GetString() ?? string.Empty
        };

        return customError;
    }

    public override void Write(Utf8JsonWriter writer, IError value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (CustomError)value, options);
    }
}