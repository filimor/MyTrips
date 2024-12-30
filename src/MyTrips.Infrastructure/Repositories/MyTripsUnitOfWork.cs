using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using MyTrips.Infrastructure.Interfaces;
using MyTrips.Infrastructure.Models;
using RepoDb;

namespace MyTrips.Infrastructure.Repositories;

public class MyTripsUnitOfWork(IOptions<AppSetting> options) : IUnitOfWork<SqlConnection>
{
    private readonly AppSetting _appSettings = options.Value;
    private SqlConnection? _connection;

    public SqlConnection Connection => _connection ?? throw new ArgumentNullException(nameof(Connection));

    public DbTransaction? Transaction { get; private set; }

    public void Begin()
    {
        if (Transaction != null)
            throw new InvalidOperationException("Cannot start a new transaction while the existing one is still open.");

        _connection = _connection ??= (SqlConnection)new SqlConnection(_appSettings.ConnectionString).EnsureOpen();

        Transaction = _connection.BeginTransaction();
    }

    public void Commit()
    {
        if (Transaction == null) throw new InvalidOperationException("There is no active transaction to commit.");

        using (Transaction)
        {
            Transaction.Commit();
        }

        Transaction = null;
    }

    public void Rollback()
    {
        if (Transaction == null) throw new InvalidOperationException("There is no active transaction to rollback.");

        using (Transaction)
        {
            Transaction.Rollback();
        }

        Transaction = null;
    }
}