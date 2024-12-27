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

    public async Task<Result<ResponseClientDto>> AddNewClientAsync(CreateClientDto createClientDto)
    {
        var existingClients = await clientsRepository.FindAsync(c => c.Email == createClientDto.Email);

        if (existingClients.Any())
            return Result.Fail(
                $"{nameof(Client)} with the {nameof(Client.Email)} '{createClientDto.Email}' already exists.");

        var client = mapper.Map<Client>(createClientDto);

        var clientId = await clientsRepository.AddAsync(client);

        client.Id = clientId;

        var clientDto = mapper.Map<ResponseClientDto>(client);

        return Result.Ok(clientDto);
    }

    public async Task<Result<ResponseClientDto>> UpdateClientAsync(UpdateClientDto updateClientDto)
    {
        var existingClients = await clientsRepository.FindAsync(c => c.Email == updateClientDto.Email);

        if (existingClients.Any())
            return Result.Fail(
                $"{nameof(Client)} with the {nameof(Client.Email)} '{updateClientDto.Email}' already exists.");

        var requestClient = mapper.Map<Client>(updateClientDto);

        var responseClient = await clientsRepository.UpdateAsync(requestClient);

        var responseClientDto = mapper.Map<ResponseClientDto>(responseClient);

        return Result.Ok(responseClientDto);
    }
}