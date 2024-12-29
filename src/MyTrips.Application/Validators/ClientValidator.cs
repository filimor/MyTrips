using System.Net.Mail;
using FluentValidation;
using MyTrips.Domain.Entities;

namespace MyTrips.Application.Validators;

public class ClientValidator : AbstractValidator<Client>
{
    public ClientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(100)
            .Must(BeAValidEmail);
    }

    private static bool BeAValidEmail(string email)
    {
        if (MailAddress.TryCreate(email, out var mailAddress))
            return mailAddress.Address == email;

        return false;
    }
}