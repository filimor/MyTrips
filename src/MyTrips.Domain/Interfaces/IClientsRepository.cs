using System.Linq.Expressions;
using MyTrips.Domain.Entities;
using MyTrips.Domain.ValueObjects;

namespace MyTrips.Domain.Interfaces;

public interface IClientsRepository
{
    Task<IEnumerable<Client>> GetAsync();
    Task<Client?> GetAsync(int id);
    Task<PagedList<Client>> GetAsync(int pageIndex, int rowsPerBatch);
    Task<int> AddAsync(Client client);
    Task<Client?> UpdateAsync(Client client);
    Task<IEnumerable<Client>> FindAsync(Expression<Func<Client, bool>> predicate);
    Task<int> DeleteAsync(int id);
}