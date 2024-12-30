using System.Data.Common;
using System.Linq.Expressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;
using MyTrips.Domain.ValueObjects;
using MyTrips.Infrastructure.Interfaces;
using MyTrips.Infrastructure.Models;
using RepoDb;
using RepoDb.Enumerations;

namespace MyTrips.Infrastructure.Repositories;

public class RepositoryBase<TDbConnection>(
    IOptions<AppSetting> settings,
    //ICache cache,
    IMyTripsTrace trace
)
    : IRepositoryBase
    where TDbConnection : DbConnection
{
    public IMyTripsTrace Trace { get; } = trace;
    //public ICache Cache { get; } = cache;

    public async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(string? cacheKey = null) where TEntity : BaseEntity
    {
        await using var connection = GetConnection();

        return await connection.QueryAllAsync<TEntity>(
            //cacheKey: cacheKey,
            commandTimeout: settings.Value.CommandTimeout,
            //cache: Cache,
            //cacheItemExpiration: settings.Value.CacheItemExpiration,
            trace: Trace
        );
    }

    public async Task<TEntity?> GetAsync<TEntity>(int id) where TEntity : BaseEntity
    {
        await using var connection = GetConnection();

        IEnumerable<TEntity?> entities =
            await connection.QueryAsync<TEntity>(id, commandTimeout: settings.Value.CommandTimeout, trace: Trace);

        return entities.FirstOrDefault();
    }

    public async Task<int> AddAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        await using var connection = GetConnection();

        return await connection.InsertAsync<TEntity, int>(entity, commandTimeout: settings.Value.CommandTimeout,
            trace: Trace);
    }

    public async Task<int> UpdateAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        await using var connection = GetConnection();

        return await connection.UpdateAsync(entity, trace: Trace);
    }

    public async Task<int> DeleteAsync<TEntity>(int id) where TEntity : BaseEntity
    {
        await using var connection = GetConnection();

        int deletedRows;

        try
        {
            deletedRows = await connection.DeleteAsync<TEntity>(id
                ,
                commandTimeout: settings.Value.CommandTimeout,
                trace: Trace
            );
        }
        catch (SqlException e) when (e.Number == 547)
        {
            return -1;
        }

        return deletedRows;
    }

    public async Task<object> MergeAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        await using var connection = GetConnection();

        return await connection.MergeAsync(entity, commandTimeout: settings.Value.CommandTimeout, trace: Trace);
    }

    public async Task<IEnumerable<TEntity>> FindAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : BaseEntity
    {
        await using var connection = GetConnection();

        return await connection.QueryAsync(predicate);
    }

    public async Task<PagedList<TEntity>> GetAsync<TEntity>(int pageIndex, int rowsPerBatch) where TEntity : BaseEntity
    {
        await using var connection = GetConnection();

        var orderBy = OrderField.Parse(new { Id = Order.Ascending });
        var page = pageIndex - 1;

        var clients = (await connection.BatchQueryAsync<TEntity>(
            page,
            rowsPerBatch,
            orderBy,
            e => e.Id > 0)).ToList();

        var count = await connection.CountAllAsync<TEntity>();

        return new PagedList<TEntity>(clients, pageIndex, (int)count, rowsPerBatch);
    }

    public TDbConnection GetConnection()
    {
        var connection = Activator.CreateInstance<TDbConnection>();

        connection.ConnectionString = settings.Value.ConnectionString;

        return connection;
    }
}