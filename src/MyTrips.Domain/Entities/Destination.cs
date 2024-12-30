namespace MyTrips.Domain.Entities;

public class Destination : BaseEntity
{
    public Destination()
    {
    }

    public Destination(string name) : this(0, name)
    {
        Name = name;
    }

    public Destination(int id, string name) : base(id)
    {
        Id = id;
    }

    public string Name { get; set; } = null!;
}