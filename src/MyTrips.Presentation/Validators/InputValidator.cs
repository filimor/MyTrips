using FluentValidation;
using FluentValidation.Results;
using MyTrips.Domain.Entities;

namespace MyTrips.Presentation.Validators;

public class InputValidator : AbstractValidator<Client>
{
    public static ValidationResult ValidateId(int id)
    {
        var idValidator = new InlineValidator<int>();
        idValidator.RuleFor(i => i).GreaterThanOrEqualTo(1).WithName(nameof(Client.Id));

        return idValidator.Validate(id);
    }
}