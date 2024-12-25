using FluentResults;
using MyTrips.Domain.Entities;

namespace MyTrips.Application.Interfaces;

public interface IClientsService
{
    Task<Result<IEnumerable<Client>>> GetClientsAsync();
}