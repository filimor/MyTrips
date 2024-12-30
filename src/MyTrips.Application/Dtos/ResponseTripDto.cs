using MyTrips.Domain.Entities;

namespace MyTrips.Application.Dtos;

public class ResponseTripDto
{
    public int Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int ClientId { get; set; }
    public int InboundFlightId { get; set; }
    public int OutboundFlightId { get; set; }
    public int HotelId { get; set; }

    public required Client Client { get; set; }
    public required Flight InboundFlight { get; set; }
    public required Flight OutboundFlight { get; set; }
    public required Hotel Hotel { get; set; }
}