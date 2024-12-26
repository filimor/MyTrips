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

    public Task<int> AddAsync(Client client)
    {
        throw new NotImplementedException();
    }
}