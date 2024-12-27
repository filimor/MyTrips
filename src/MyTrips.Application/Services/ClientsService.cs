using AutoMapper;
using FluentResults;
using MyTrips.Application.Dtos;
using MyTrips.Application.Interfaces;
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;

namespace MyTrips.Application.Services;

public class ClientsService(IMapper mapper, IClientsRepository clientsRepository) : IClientsService
{
    public async Task<Result<IEnumerable<ResponseClientDto>>> GetClientsAsync()
    {
        var clients = await clientsRepository.GetAsync();
        var clientsDto = mapper.Map<IEnumerable<ResponseClientDto>>(clients);

        return Result.Ok(clientsDto);
    }

    public async Task<Result<ResponseClientDto>> GetClientByIdAsync(int id)
    {
        var client = await clientsRepository.GetAsync(id);

        if (client is null)
            return Result.Fail($"{nameof(Client)} with {nameof(Client.Id)} '{id}' not found.");

        var clientDto = mapper.Map<ResponseClientDto>(client);

        return Result.Ok(clientDto);
    }

    public async Task<Result<ResponseClientDto>> AddNewClientAsync(RequestClientDto requestClientDto)
    {
        var existingClients = await clientsRepository.FindAsync(c => c.Email == requestClientDto.Email);

        if (existingClients.Any())
            return Result.Fail(
                $"{nameof(Client)} with the {nameof(Client.Email)} '{requestClientDto.Email}' already exists.");

        var client = mapper.Map<Client>(requestClientDto);

        var clientId = await clientsRepository.AddAsync(client);

        client.Id = clientId;

        var clientDto = mapper.Map<ResponseClientDto>(client);

        return Result.Ok(clientDto);
    }
}