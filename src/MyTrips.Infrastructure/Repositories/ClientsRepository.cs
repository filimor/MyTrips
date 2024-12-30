using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using MyTrips.Domain.Interfaces;
using MyTrips.Infrastructure.Models;

namespace MyTrips.Infrastructure.Repositories;

public class ClientsRepository(IOptions<AppSetting> settings)
    : RepositoryBase<SqlConnection>(settings), IClientsRepository
{
}