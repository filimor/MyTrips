using MyTrips.Domain.Entities;

namespace MyTrips.Domain.Interfaces;

public interface IClientsRepository
{
    string ConnectionString { get; }
    Task<IEnumerable<Client>> GetAsync();
    Task<Client?> GetAsync(int id);
    Task<int> AddAsync(Client client);
}