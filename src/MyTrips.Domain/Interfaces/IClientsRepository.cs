using MyTrips.Domain.Entities;

namespace MyTrips.Domain.Interfaces;

public interface IClientsRepository
{
    Task<IEnumerable<Client>> GetAsync();
}