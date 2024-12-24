using MyTrips.Domain.Entities;

namespace MyTrips.Domain.Interfaces;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetAsync();
}