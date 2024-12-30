using FluentResults;
using MyTrips.Application.Dtos;
using MyTrips.Domain.ValueObjects;

namespace MyTrips.Application.Interfaces;

public interface ITripsService
{
    Task<Result<IEnumerable<ShortResponseTripDto>>> GetAllTripsAsync();
    Task<Result<PagedList<ShortResponseTripDto>>> GetTripsAsync(GetParameters parameters);
    Task<Result<ResponseTripDto>> GetTripByIdAsync(int id);
    Task<Result<ResponseTripDto>> BookTripAsync(CreateTripDto createTripDto);
    Task<Result> CancelTripAsync(int id);
}