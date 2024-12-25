using FluentResults;
using FluentValidation;
using FluentValidation.Results;
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
        var validationResult = new ClientValidator().ValidateId(id);

        if (id < 1)
        {
            var errors = validationResult.Errors.Select(e => new Error(e.ErrorMessage));
            return Result.Fail(errors);
        }

        return Result.Ok(await clientsRepository.GetAsync(id));
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