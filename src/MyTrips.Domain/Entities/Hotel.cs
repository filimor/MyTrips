namespace MyTrips.Domain.Entities;

public class Hotel : BaseEntity
{
    public Hotel()
    {
    }

    public Hotel(string name, int rating, decimal price, int destinationId) : this(0, name, rating, price,
        destinationId)
    {
    }

    public Hotel(int id, string name, int rating, decimal price, int destinationId) : base(id)
    {
        Name = name;
        Rating = rating;
        Price = price;
        DestinationId = destinationId;
    }

    public string Name { get; set; } = null!;
    public int Rating { get; set; }
    public decimal Price { get; set; }
    public int DestinationId { get; set; }

    public Destination Destination { get; set; } = null!;
}