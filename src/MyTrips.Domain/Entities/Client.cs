namespace MyTrips.Domain.Entities;

public class Client
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
}