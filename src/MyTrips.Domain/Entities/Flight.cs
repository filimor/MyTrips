namespace MyTrips.Domain.Entities;

public class Flight : BaseEntity
{
    public Flight()
    {
    }

    public Flight(string flightNumber, string departureAirport, string arrivalAirport, DateOnly departureDate,
        DateOnly arrivalDate) : this(0, flightNumber, departureAirport, arrivalAirport, departureDate, arrivalDate)
    {
    }

    public Flight(int id, string flightNumber, string departureAirport, string arrivalAirport, DateOnly departureDate,
        DateOnly arrivalDate) : base(id)
    {
        FlightNumber = flightNumber;
        DepartureAirport = departureAirport;
        ArrivalAirport = arrivalAirport;
        DepartureDate = departureDate;
        ArrivalDate = arrivalDate;
    }

    public string FlightNumber { get; set; } = null!;
    public string DepartureAirport { get; set; } = null!;
    public string ArrivalAirport { get; set; } = null!;
    public DateOnly DepartureDate { get; set; }
    public DateOnly ArrivalDate { get; set; }
}