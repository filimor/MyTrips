namespace MyTrips.Domain.Entities;

public class Client
{
    public Client()
    {
    }

    public Client(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public Client(int id, string name, string email) : this(name, email)
    {
        Id = id;
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
}