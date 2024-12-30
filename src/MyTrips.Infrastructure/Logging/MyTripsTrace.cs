using MyTrips.Infrastructure.Interfaces;
using RepoDb;
using Serilog;

namespace MyTrips.Infrastructure.Logging;

public class MyTripsTrace : IMyTripsTrace
{
    public void BeforeExecution(CancellableTraceLog log)
    {
        Log.Debug(
            "SQL Query Started:\nKey: {Key}\nSession: {Session}\nStart time: {StartTime}\nStatement: {Statement}\nParameters: {Parameters}",
            log.StartTime, log.Key, log.SessionId, log.Statement, log.Parameters);
    }

    public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
    {
        Log.Debug("SQL Query Ended:\nKey: {Key}\nSession: {Session}\nExecution time: {ExecutionTime}\nResult: {Result}",
            log.Key, log.SessionId, log.ExecutionTime, log.Result);
    }

    public Task BeforeExecutionAsync(CancellableTraceLog log, CancellationToken cancellationToken = new())
    {
        Log.Debug(
            "SQL Query Started:\nKey: {Key}\nSession: {Session}\nStart time: {StartTime}\nStatement: {Statement}\nParameters: {Parameters}",
            log.StartTime, log.Key, log.SessionId, log.Statement, log.Parameters);

        return Task.CompletedTask;
    }

    public Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log, CancellationToken cancellationToken = new())
    {
        Log.Debug("SQL Query Ended:\nKey: {Key}\nSession: {Session}\nExecution time: {ExecutionTime}\nResult: {Result}",
            log.Key, log.SessionId, log.ExecutionTime, log.Result);

        return Task.CompletedTask;
    }
}