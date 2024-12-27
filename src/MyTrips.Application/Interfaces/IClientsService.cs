using FluentResults;
using MyTrips.Application.Dtos;

namespace MyTrips.Application.Interfaces;

public interface IClientsService
{
    Task<Result<IEnumerable<ResponseClientDto>>> GetClientsAsync();
    Task<Result<ResponseClientDto>> GetClientByIdAsync(int id);
    Task<Result<ResponseClientDto>> AddNewClientAsync(CreateClientDto dto);
    Task<Result<ResponseClientDto>> UpdateClientAsync(UpdateClientDto updateClientDto);
}