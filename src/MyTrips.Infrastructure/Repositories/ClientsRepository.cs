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
        return connection.QueryAll<Client>();
    }
}