using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;
using MyTrips.Infrastructure.Interfaces;
using MyTrips.Infrastructure.Models;
using RepoDb;

namespace MyTrips.Infrastructure.Repositories;

public class TripsRepository(IOptions<AppSetting> settings, IMyTripsTrace trace, IUnitOfWork<SqlConnection> unitOfWork)
    : RepositoryBase<SqlConnection>(settings, trace, unitOfWork), ITripsRepository
{
    private readonly IOptions<AppSetting> _settings = settings;

    public async Task<Trip?> GetAsync(int id)
    {
        await using var connection = GetConnection();

        var trip = (await connection.QueryAsync<Trip>(id, commandTimeout: _settings.Value.CommandTimeout, trace: Trace))
            .FirstOrDefault();

        if (trip is null) return null;

        await FillOutTripData(connection, trip);

        return trip;
    }

    private async Task FillOutTripData(SqlConnection connection, Trip trip)
    {
        var client = (await connection.QueryAsync<Client>(c => c.Id == trip.ClientId,
            commandTimeout: _settings.Value.CommandTimeout, trace: Trace)).FirstOrDefault();
        trip.Client = client ?? throw new InvalidOperationException("Client not found");

        var outboundFlight = (await connection.QueryAsync<Flight>(f => f.Id == trip.OutboundFlightId,
            commandTimeout: _settings.Value.CommandTimeout, trace: Trace)).FirstOrDefault();
        trip.OutboundFlight = outboundFlight ?? throw new InvalidOperationException("Outbound flight not found");

        var inboundFlight = (await connection.QueryAsync<Flight>(f => f.Id == trip.InboundFlightId,
            commandTimeout: _settings.Value.CommandTimeout, trace: Trace)).FirstOrDefault();
        trip.InboundFlight = inboundFlight ?? throw new InvalidOperationException("Inbound flight not found");

        var hotel = (await connection.QueryAsync<Hotel>(h => h.Id == trip.HotelId,
            commandTimeout: _settings.Value.CommandTimeout, trace: Trace)).FirstOrDefault();
        trip.Hotel = hotel ?? throw new InvalidOperationException("Hotel not found");

        var destination = (await connection.QueryAsync<Destination>(d => d.Id == hotel.DestinationId,
            commandTimeout: _settings.Value.CommandTimeout, trace: Trace)).FirstOrDefault();
        hotel.Destination = destination ?? throw new InvalidOperationException("Destination not found");
    }
}