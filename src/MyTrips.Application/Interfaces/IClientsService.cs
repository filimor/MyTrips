using FluentResults;
using MyTrips.Application.Dtos;
using MyTrips.Domain.ValueObjects;

namespace MyTrips.Application.Interfaces;

public interface IClientsService
{
    Task<Result<IEnumerable<ResponseClientDto>>> GetAllClientsAsync();
    Task<Result<PagedList<ResponseClientDto>>> GetClientsAsync(GetParameters parameters);
    Task<Result<ResponseClientDto>> GetClientByIdAsync(int id);
    Task<Result<ResponseClientDto>> AddNewClientAsync(CreateClientDto createClientDto);
    Task<Result<ResponseClientDto>> UpdateClientAsync(UpdateClientDto updateClientDto);
    Task<Result> RemoveClientAsync(int id);
}