using System.Linq.Expressions;
using MyTrips.Domain.Entities;
using MyTrips.Domain.ValueObjects;

namespace MyTrips.Domain.Interfaces;

public interface IClientsRepository
{
    Task<IEnumerable<Client>> GetAsync();
    Task<PagedList<Client>> GetAsync(int pageIndex, int rowsPerBatch);
    Task<Client?> GetAsync(int id);
    Task<int> AddAsync(Client client);
    Task<Client?> UpdateAsync(Client client);
    Task<int> DeleteAsync(int id);
    Task<IEnumerable<Client>> FindAsync(Expression<Func<Client, bool>> predicate);
}