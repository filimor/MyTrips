using Microsoft.Data.SqlClient;
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;
using RepoDb;

namespace MyTrips.Infrastructure.Repositories;

public class ClientsRepository : IClientsRepository
{
    public async Task<IEnumerable<Client>> GetAsync()
    {
        await using SqlConnection connection = new(
            "Data Source=mytrips.database.windows.net;Initial Catalog=MyTripsDb;User ID=dbadmin;Password=@2eJgt57MhF6Zd$!5#4#;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
        return connection.QueryAll<Client>();
    }
}