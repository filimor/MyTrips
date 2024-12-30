using AutoMapper;
using FluentResults;
using MyTrips.Application.Dtos;
using MyTrips.Application.Errors;
using MyTrips.Application.Interfaces;
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;
using MyTrips.Domain.ValueObjects;

namespace MyTrips.Application.Services;

public class ClientsService(IMapper mapper, IClientsRepository clientsRepository) : IClientsService
{
    public async Task<Result<IEnumerable<ResponseClientDto>>> GetAllClientsAsync()
    {
        var clients = await clientsRepository.GetAllAsync<Client>();
        var clientsDto = mapper.Map<IEnumerable<ResponseClientDto>>(clients);

        return Result.Ok(clientsDto);
    }

    public async Task<Result<PagedList<ResponseClientDto>>> GetClientsAsync(GetParameters parameters)
    {
        var clientsPaged = await clientsRepository.GetAsync<Client>(parameters.PageIndex, parameters.PageSize);
        var dtosPaged = mapper.Map<PagedList<ResponseClientDto>>(clientsPaged);

        return Result.Ok(dtosPaged);
    }

    public async Task<Result<ResponseClientDto>> GetClientByIdAsync(int id)
    {
        var client = await clientsRepository.GetAsync<Client>(id);

        if (client is null)
            return Result.Fail(new NotFoundError($"{nameof(Client)} with {nameof(Client.Id)} '{id}' not found."));

        var clientDto = mapper.Map<ResponseClientDto>(client);

        return Result.Ok(clientDto);
    }

    public async Task<Result<ResponseClientDto>> AddNewClientAsync(CreateClientDto createClientDto)
    {
        var existingClients = await clientsRepository.FindAsync<Client>(c => c.Email == createClientDto.Email);

        if (existingClients.Any())
            return Result.Fail(new ConflictError(
                $"{nameof(Client)} with the {nameof(Client.Email)} '{createClientDto.Email}' already exists."));

        var client = mapper.Map<Client>(createClientDto);

        var clientId = await clientsRepository.AddAsync(client);

        client.Id = clientId;

        var clientDto = mapper.Map<ResponseClientDto>(client);

        return Result.Ok(clientDto);
    }

    public async Task<Result<ResponseClientDto>> UpdateClientAsync(UpdateClientDto updateClientDto)
    {
        var existingClients = await clientsRepository.FindAsync<Client>(c => c.Email == updateClientDto.Email);

        if (existingClients.Any())
            return Result.Fail(
                new ConflictError(
                    $"{nameof(Client)} with the {nameof(Client.Email)} '{updateClientDto.Email}' already exists."));

        var requestClient = mapper.Map<Client>(updateClientDto);

        var rowsUpdated = await clientsRepository.UpdateAsync(requestClient);

        if (rowsUpdated == 0)
            return Result.Fail(
                new NotFoundError($"{nameof(Client)} with {nameof(Client.Id)} '{updateClientDto.Id}' not found."));

        var responseClientDto = mapper.Map<ResponseClientDto>(updateClientDto);

        return Result.Ok(responseClientDto);
    }

    public async Task<Result> RemoveClientAsync(int id)
    {
        var result = await clientsRepository.DeleteAsync<Client>(id);

        return result switch
        {
            0 => Result.Fail(new NotFoundError($"{nameof(Client)} with {nameof(Client.Id)} '{id}' not found.")),
            -1 => Result.Fail(new ConflictError(
                $"The {nameof(Client)} with {nameof(Client.Id)} '{id}' is referenced by another entity.")),
            _ => Result.Ok()
        };
    }
}