using System.Linq.Expressions;
using MyTrips.Domain.Entities;
using MyTrips.Domain.ValueObjects;

namespace MyTrips.Domain.Interfaces;

public interface IRepositoryBase
{
    Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(string? cacheKey = null) where TEntity : BaseEntity;
    Task<PagedList<TEntity>> GetAsync<TEntity>(int pageIndex, int rowsPerBatch) where TEntity : BaseEntity;
    Task<TEntity?> GetAsync<TEntity>(int id) where TEntity : BaseEntity;
    Task<int> AddAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;
    Task<int> UpdateAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;
    Task<int> DeleteAsync<TEntity>(int id) where TEntity : BaseEntity;
    Task<object> MergeAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;
    Task<IEnumerable<TEntity>> FindAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : BaseEntity;
}