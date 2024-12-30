using System.Text.Json.Serialization;

namespace MyTrips.Application.Dtos;

public class ShortResponseTripDto
{
    [JsonPropertyOrder(1)] public int Id { get; set; }

    [JsonPropertyOrder(2)] public DateOnly StartDate { get; set; }

    [JsonPropertyOrder(3)] public DateOnly EndDate { get; set; }

    [JsonPropertyOrder(4)] public int ClientId { get; set; }

    [JsonPropertyOrder(5)] public int InboundFlightId { get; set; }

    [JsonPropertyOrder(6)] public int OutboundFlightId { get; set; }

    [JsonPropertyOrder(7)] public int HotelId { get; set; }
}