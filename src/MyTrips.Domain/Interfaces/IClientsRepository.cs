using System.Linq.Expressions;
using MyTrips.Domain.Entities;

namespace MyTrips.Domain.Interfaces;

public interface IClientsRepository
{
    Task<IEnumerable<Client>> GetAsync();
    Task<Client?> GetAsync(int id);
    Task<int> AddAsync(Client client);
    Task<IEnumerable<Client>> FindAsync(Expression<Func<Client, bool>> predicate);
}