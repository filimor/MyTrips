using System.Text.Json.Serialization;

namespace MyTrips.Application.Dtos;

public class CreateClientDto
{
    [JsonPropertyOrder(1)] public string Name { get; set; } = null!;

    [JsonPropertyOrder(2)] public string Email { get; set; } = null!;
}