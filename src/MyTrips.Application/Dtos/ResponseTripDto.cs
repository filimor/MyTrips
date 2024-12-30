using System.Text.Json.Serialization;

namespace MyTrips.Application.Dtos;

public class ResponseTripDto
{
    [JsonPropertyOrder(1)] public int Id { get; set; }

    [JsonPropertyOrder(2)] public DateOnly StartDate { get; set; }

    [JsonPropertyOrder(3)] public DateOnly EndDate { get; set; }

    [JsonIgnore] public int ClientId { get; set; }

    [JsonIgnore] public int InboundFlightId { get; set; }
    [JsonIgnore] public int OutboundFlightId { get; set; }
    [JsonIgnore] public int HotelId { get; set; }

    [JsonPropertyOrder(4)] public required ResponseClientDto Client { get; set; }

    [JsonPropertyOrder(5)] public required ResponseFlightDto OutboundFlight { get; set; }

    [JsonPropertyOrder(6)] public required ResponseFlightDto InboundFlight { get; set; }

    [JsonPropertyOrder(7)] public required ResponseHotelDto Hotel { get; set; }
}