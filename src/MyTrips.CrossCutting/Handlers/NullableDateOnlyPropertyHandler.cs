using RepoDb.Interfaces;
using RepoDb.Options;

namespace MyTrips.CrossCutting.Handlers;

public class NullableDateOnlyPropertyHandler : IPropertyHandler<DateTime?, DateOnly?>
{
    public DateOnly? Get(DateTime? dt, PropertyHandlerGetOptions options)
    {
        return dt.HasValue ? DateOnly.FromDateTime(dt.Value) : null;
    }

    public DateTime? Set(DateOnly? input, PropertyHandlerSetOptions options)
    {
        return input?.ToDateTime(TimeOnly.MinValue);
    }
}