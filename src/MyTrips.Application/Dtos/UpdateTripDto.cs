﻿namespace MyTrips.Application.Dtos;

public class UpdateTripDto
{
    public int Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int ClientId { get; set; }
    public int InboundFlightId { get; set; }
    public int OutboundFlightId { get; set; }
    public int HotelId { get; set; }
}