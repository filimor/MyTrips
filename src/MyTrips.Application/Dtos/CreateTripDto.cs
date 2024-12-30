using System.Text.Json.Serialization;

namespace MyTrips.Application.Dtos;

public class CreateTripDto
{
    [JsonPropertyOrder(1)] public DateOnly StartDate { get; set; }

    [JsonPropertyOrder(2)] public DateOnly EndDate { get; set; }

    [JsonPropertyOrder(3)] public int ClientId { get; set; }

    [JsonPropertyOrder(4)] public int InboundFlightId { get; set; }

    [JsonPropertyOrder(5)] public int HotelId { get; set; }

    [JsonPropertyOrder(6)] public int OutboundFlightId { get; set; }
}