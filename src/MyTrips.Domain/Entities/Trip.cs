namespace MyTrips.Domain.Entities;

public class Trip : BaseEntity
{
    public Trip()
    {
    }

    public Trip(DateOnly startDate, DateOnly endDate, int clientId, int inboundFlightId, int outboundFlightId,
        int hotelId) : this(0, startDate, endDate, clientId, inboundFlightId, outboundFlightId, hotelId)
    {
    }

    public Trip(int id, DateOnly startDate, DateOnly endDate, int clientId, int inboundFlightId, int outboundFlightId,
        int hotelId) : base(id)
    {
        StartDate = startDate;
        EndDate = endDate;
        ClientId = clientId;
        InboundFlightId = inboundFlightId;
        OutboundFlightId = outboundFlightId;
        HotelId = hotelId;
    }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int ClientId { get; set; }
    public int InboundFlightId { get; set; }
    public int OutboundFlightId { get; set; }
    public int HotelId { get; set; }

    public Client Client { get; set; } = null!;
    public Flight InboundFlight { get; set; } = null!;
    public Flight OutboundFlight { get; set; } = null!;
    public Hotel Hotel { get; set; } = null!;

    public decimal GetTotalPrice()
    {
        return 0;
    }
}