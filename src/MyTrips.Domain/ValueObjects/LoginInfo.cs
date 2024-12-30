using System.Text.Json.Serialization;

namespace MyTrips.Domain.ValueObjects;

public class LoginInfo
{
    [JsonPropertyOrder(1)] public string Username { get; set; } = null!;

    [JsonPropertyOrder(2)] public string Password { get; set; } = null!;
}