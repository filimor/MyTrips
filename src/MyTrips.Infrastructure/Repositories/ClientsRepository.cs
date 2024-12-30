using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using MyTrips.Domain.Interfaces;
using MyTrips.Infrastructure.Interfaces;
using MyTrips.Infrastructure.Models;

namespace MyTrips.Infrastructure.Repositories;

public class ClientsRepository(
    IOptions<AppSetting> settings,
    IMyTripsTrace trace,
    IUnitOfWork<SqlConnection> unitOfWork)
    : RepositoryBase<SqlConnection>(settings, trace, unitOfWork), IClientsRepository
{
}