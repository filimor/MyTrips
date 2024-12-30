using System.Data.Common;

namespace MyTrips.Infrastructure.Interfaces;

public interface IUnitOfWork<out TDbConnection>
{
    TDbConnection Connection { get; }
    DbTransaction? Transaction { get; }
    void Begin();
    void Rollback();
    void Commit();
}