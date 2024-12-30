using System.Text.Json.Serialization;

namespace MyTrips.Application.Dtos;

public class ResponseFlightDto
{
    [JsonPropertyOrder(1)] public int Id { get; set; }

    [JsonPropertyOrder(2)] public string FlightNumber { get; set; } = null!;

    [JsonPropertyOrder(3)] public string DepartureAirport { get; set; } = null!;

    [JsonPropertyOrder(4)] public string ArrivalAirport { get; set; } = null!;

    [JsonPropertyOrder(5)] public DateTime DepartureDateTime { get; set; }

    [JsonPropertyOrder(6)] public DateTime ArrivalDateTime { get; set; }
}