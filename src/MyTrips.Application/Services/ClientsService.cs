using AutoMapper;
using FluentResults;
using MyTrips.Application.Dtos;
using MyTrips.Application.Interfaces;
using MyTrips.Domain.Interfaces;

namespace MyTrips.Application.Services;

public class ClientsService(IMapper mapper, IClientsRepository clientsRepository) : IClientsService
{
    public async Task<Result<IEnumerable<ClientDto>>> GetClientsAsync()
    {
        var clients = await clientsRepository.GetAsync();
        var clientsDto = mapper.Map<IEnumerable<ClientDto>>(clients);

        return Result.Ok(clientsDto);
    }

    public async Task<Result<ClientDto>> GetClientByIdAsync(int id)
    {
        var client = await clientsRepository.GetAsync(id);

        if (client is null)
            return Result.Fail(new Error($"Client with id '{id}' not found."));

        var clientDto = mapper.Map<ClientDto>(client);

        return Result.Ok(clientDto);
    }
}