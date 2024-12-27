using FluentValidation;
using MyTrips.Domain.Entities;

namespace MyTrips.Application.Validators;

public class ClientValidator : AbstractValidator<Client>
{
    public ClientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        //RuleFor(x => x.Email)
        //    .NotEmpty().WithMessage("Email is required.")
        //    .MaximumLength(100).WithMessage("Email must not exceed 100 characters.")
        //    .EmailAddress().WithMessage("Invalid email address.");
    }
}