namespace MyTrips.Application.Dtos;

public class UpdateClientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
}