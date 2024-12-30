using System.Text.Json.Serialization;

namespace MyTrips.Application.Dtos;

public class ResponseDestinationDto
{
    [JsonPropertyOrder(1)] public int Id { get; set; }

    [JsonPropertyOrder(2)] public string Name { get; set; } = null!;
}