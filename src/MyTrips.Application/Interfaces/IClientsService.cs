using FluentResults;
using MyTrips.Application.Dtos;

namespace MyTrips.Application.Interfaces;

public interface IClientsService
{
    // TODO: Change it to DTO
    Task<Result<IEnumerable<ClientDto>>> GetClientsAsync();
    Task<Result<ClientDto>> GetClientByIdAsync(int id);
}