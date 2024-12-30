namespace MyTrips.Domain.Entities;

public class Client : BaseEntity
{
    public Client()
    {
    }

    public Client(string name, string email) : this(0, name, email)
    {
    }

    public Client(int id, string name, string email) : base(id)
    {
        Name = name;
        Email = email;
    }

    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
}