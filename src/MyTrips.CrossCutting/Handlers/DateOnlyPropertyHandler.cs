using RepoDb.Interfaces;
using RepoDb.Options;

namespace MyTrips.CrossCutting.Handlers;

public class DateOnlyPropertyHandler : IPropertyHandler<DateTime, DateOnly>
{
    public DateOnly Get(DateTime input, PropertyHandlerGetOptions options)
    {
        return DateOnly.FromDateTime(input);
    }

    public DateTime Set(DateOnly input, PropertyHandlerSetOptions options)
    {
        return input.ToDateTime(TimeOnly.MinValue);
    }
}