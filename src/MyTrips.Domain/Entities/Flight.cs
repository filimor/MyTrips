namespace MyTrips.Domain.Entities;

public class Flight : BaseEntity
{
    public Flight()
    {
    }

    public Flight(string flightNumber, string departureAirport, string arrivalAirport, DateTime departureDateTime,
        DateTime arrivalDateTime) : this(0, flightNumber, departureAirport, arrivalAirport, departureDateTime,
        arrivalDateTime)
    {
    }

    public Flight(int id, string flightNumber, string departureAirport, string arrivalAirport,
        DateTime departureDateTime,
        DateTime arrivalDateTime) : base(id)
    {
        FlightNumber = flightNumber;
        DepartureAirport = departureAirport;
        ArrivalAirport = arrivalAirport;
        DepartureDateTime = departureDateTime;
        ArrivalDateTime = arrivalDateTime;
    }

    public string FlightNumber { get; set; } = null!;
    public string DepartureAirport { get; set; } = null!;
    public string ArrivalAirport { get; set; } = null!;
    public DateTime DepartureDateTime { get; set; }
    public DateTime ArrivalDateTime { get; set; }
}