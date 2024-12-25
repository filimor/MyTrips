using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using MyTrips.Application.Dtos;
using MyTrips.Application.Interfaces;
using MyTrips.Domain.Entities;
using MyTrips.Domain.Interfaces;

namespace MyTrips.Application.Services;

public class ClientsService(IMapper mapper, IClientsRepository clientsRepository) : IClientsService
{
    public async Task<Result<IEnumerable<Client>>> GetClientsAsync()
    {
        return Result.Ok(await clientsRepository.GetAsync());
    }

    public async Task<Result<ClientDto>> GetClientByIdAsync(int id)
    {
        var validationResult = new ClientValidator().ValidateId(id);

        if (id < 1)
        {
            var errors = validationResult.Errors.Select(e => new Error(e.ErrorMessage));
            return Result.Fail(errors);
        }

        var client = await clientsRepository.GetAsync(id);

        if (client is null)
            // TODO: Must use standard error messages
            return Result.Fail(new Error($"Client with id '{id}' not found."));

        var clientDto = mapper.Map<ClientDto>(client);

        return Result.Ok(clientDto);
    }
}

public class ClientValidator : AbstractValidator<Client>
{
    public ValidationResult ValidateId(int id)
    {
        var idValidator = new InlineValidator<int>();
        idValidator.RuleFor(i => i).GreaterThanOrEqualTo(1).WithName(nameof(Client.Id));

        return idValidator.Validate(id);
    }
}