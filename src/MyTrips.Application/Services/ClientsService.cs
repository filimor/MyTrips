using FluentResults;
using MyTrips.Application.Interfaces;
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;

namespace MyTrips.Application.Services;

public class ClientsService(IClientsRepository clientsRepository) : IClientsService
{
    public async Task<Result<IEnumerable<Client>>> GetClientsAsync()
    {
        return Result.Ok(await clientsRepository.GetAsync());
    }

    public async Task<Result<Client>> GetClientByIdAsync(int id)
    {
        if (id < 1) return Result.Fail($"Invalid id: {id}");
        return Result.Ok(await clientsRepository.GetAsync(id));
    }
}