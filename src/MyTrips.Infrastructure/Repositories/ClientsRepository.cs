using System.Linq.Expressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;
using RepoDb;

namespace MyTrips.Infrastructure.Repositories;

public class ClientsRepository(IConfiguration configuration) : IClientsRepository
{
    public string ConnectionString { get; } = configuration.GetConnectionString("MyTripsDb") ??
                                              throw new ArgumentNullException(nameof(ConnectionString));

    public async Task<IEnumerable<Client>> GetAsync()
    {
        await using SqlConnection connection = new(ConnectionString);

        return await connection.QueryAllAsync<Client>();
    }

    public async Task<Client?> GetAsync(int id)
    {
        await using SqlConnection connection = new(ConnectionString);

        var clients = await connection.QueryAsync<Client>(e => e.Id == id);

        return clients.FirstOrDefault();
    }

    public async Task<int> AddAsync(Client client)
    {
        await using SqlConnection connection = new(ConnectionString);

        var id = await connection.InsertAsync<Client, int>(client);

        return id;
    }

    public async Task<Client?> UpdateAsync(Client client)
    {
        await using SqlConnection connection = new(ConnectionString);

        var updatedRows = await connection.UpdateAsync(client);

        return updatedRows > 0 ? client : null;
    }

    public async Task<IEnumerable<Client>> FindAsync(Expression<Func<Client, bool>> predicate)
    {
        await using SqlConnection connection = new(ConnectionString);

        return await connection.QueryAsync(predicate);
    }

    public async Task<int> DeleteAsync(int id)
    {
        await using SqlConnection connection = new(ConnectionString);

        int deletedRows;

        try
        {
            deletedRows = await connection.DeleteAsync<Client>(id);
        }
        catch (SqlException e) when (e.Number == 547)
        {
            return -1;
        }


        return deletedRows;
    }
}