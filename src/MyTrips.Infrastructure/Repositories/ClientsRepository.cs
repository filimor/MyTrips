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
            "Server=tcp:mytrips.database.windows.net,1433;Initial Catalog=MyTripsDb;Persist Security Info=False;User ID=dbadmin;Password=@2eJgt57MhF6Zd$!5#4#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        return connection.QueryAll<Client>();
    }
}