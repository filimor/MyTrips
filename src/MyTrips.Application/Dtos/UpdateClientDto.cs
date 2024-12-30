using System.Text.Json.Serialization;

namespace MyTrips.Application.Dtos;

public class UpdateClientDto
{
    [JsonPropertyOrder(1)] public int Id { get; set; }

    [JsonPropertyOrder(2)] public string Name { get; set; } = null!;

    [JsonPropertyOrder(3)] public string Email { get; set; } = null!;
}