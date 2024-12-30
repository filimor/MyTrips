using RepoDb;
using RepoDb.Interfaces;

namespace MyTrips.Infrastructure.Models;

internal class CustomTrace : ITrace
{
    public void BeforeExecution(CancellableTraceLog log)
    {
        throw new NotImplementedException();
    }

    public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
    {
        throw new NotImplementedException();
    }

    public Task BeforeExecutionAsync(CancellableTraceLog log, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }
}