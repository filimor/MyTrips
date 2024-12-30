using AutoMapper;
using FluentResults;
using MyTrips.Application.Dtos;
using MyTrips.Application.Errors;
using MyTrips.Application.Interfaces;
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;
using MyTrips.Domain.ValueObjects;

namespace MyTrips.Application.Services;

public class TripsService(IMapper mapper, ITripsRepository tripsRepository) : ITripsService
{
    public async Task<Result<IEnumerable<ShortResponseTripDto>>> GetAllTripsAsync()
    {
        var trips = await tripsRepository.GetAllAsync<Trip>();
        var tripsDto = mapper.Map<IEnumerable<ShortResponseTripDto>>(trips);

        return Result.Ok(tripsDto);
    }

    public async Task<Result<PagedList<ShortResponseTripDto>>> GetTripsAsync(GetParameters parameters)
    {
        var tripsPaged = await tripsRepository.GetAsync<Trip>(parameters.PageIndex, parameters.PageSize);
        var dtosPaged = mapper.Map<PagedList<ShortResponseTripDto>>(tripsPaged);

        return Result.Ok(dtosPaged);
    }

    public async Task<Result<ResponseTripDto>> GetTripByIdAsync(int id)
    {
        var trip = await tripsRepository.GetAsync(id);

        if (trip is null)
            return Result.Fail(new NotFoundError($"{nameof(Trip)} with {nameof(Trip.Id)} '{id}' not found."));

        var tripDto = mapper.Map<ResponseTripDto>(trip);

        return Result.Ok(tripDto);
    }

    public async Task<Result<ResponseTripDto>> BookTripAsync(CreateTripDto createTripDto)
    {
        //if (await IsConflictingTrip(createTripDto))
        //    return Result.Fail(new ConflictError(
        //        "There's already a trip scheduled within that interval."));

        var trip = mapper.Map<Trip>(createTripDto);

        var client = await tripsRepository.GetAsync<Client>(trip.ClientId);
        if (client is null)
            return Result.Fail(new NotFoundError(
                $"{nameof(Client)} with {nameof(Client.Id)} '{trip.ClientId}' not found."));

        var outboundFlight = await tripsRepository.GetAsync<Flight>(trip.OutboundFlightId);
        if (outboundFlight is null)
            return Result.Fail(new NotFoundError(
                $"{nameof(Flight)} with {nameof(Flight.Id)} '{trip.OutboundFlightId}' not found."));

        var inboundFlight = await tripsRepository.GetAsync<Flight>(trip.InboundFlightId);
        if (inboundFlight is null)
            return Result.Fail(new NotFoundError(
                $"{nameof(Flight)} with {nameof(Flight.Id)} '{trip.InboundFlightId}' not found."));

        var hotel = await tripsRepository.GetAsync<Hotel>(trip.HotelId);
        if (hotel is null)
            return Result.Fail(new NotFoundError(
                $"{nameof(Hotel)} with {nameof(Hotel.Id)} '{trip.HotelId}' not found."));

        var tripId = await tripsRepository.AddAsync(trip);

        var createdTrip = await tripsRepository.GetAsync(tripId);

        var tripDto = mapper.Map<ResponseTripDto>(createdTrip);

        return Result.Ok(tripDto);
    }

    public async Task<Result> CancelTripAsync(int id)
    {
        var result = await tripsRepository.DeleteAsync<Trip>(id);

        return result switch
        {
            0 => Result.Fail(new NotFoundError($"{nameof(Trip)} with {nameof(Trip.Id)} '{id}' not found.")),
            _ => Result.Ok()
        };
    }
}