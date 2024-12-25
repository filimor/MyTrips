using FluentResults;
using MyTrips.Application.Dtos;
using MyTrips.Domain.Entities;

namespace MyTrips.Application.Interfaces;

public interface IClientsService
{
    // TODO: Change it to DTO
    Task<Result<IEnumerable<Client>>> GetClientsAsync();
    Task<Result<ClientDto>> GetClientByIdAsync(int id);
}