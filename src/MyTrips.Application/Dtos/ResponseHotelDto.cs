using System.Text.Json.Serialization;

namespace MyTrips.Application.Dtos;

public class ResponseHotelDto
{
    [JsonPropertyOrder(1)] public int Id { get; set; }

    [JsonPropertyOrder(2)] public string Name { get; set; } = null!;

    [JsonPropertyOrder(3)] public int Rating { get; set; }

    [JsonPropertyOrder(4)] public decimal Price { get; set; }

    [JsonIgnore] public int DestinationId { get; set; }

    [JsonPropertyOrder(5)] public ResponseDestinationDto Destination { get; set; } = null!;
}