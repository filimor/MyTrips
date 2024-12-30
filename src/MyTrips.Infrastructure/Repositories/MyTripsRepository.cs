using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using MyTrips.Infrastructure.Models;
using RepoDb.Interfaces;

namespace MyTrips.Infrastructure.Repositories;

public class MyTripsRepository(IOptions<AppSetting> settings, ICache cache, ITrace trace)
    : RepositoryBase<SqlConnection>(settings
        //, cache, trace
    )
{
}