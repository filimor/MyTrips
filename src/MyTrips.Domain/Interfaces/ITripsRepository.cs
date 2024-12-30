using MyTrips.Domain.Entities;

namespace MyTrips.Domain.Interfaces;

public interface ITripsRepository : IRepositoryBase
{
    Task<Trip?> GetAsync(int id);
}