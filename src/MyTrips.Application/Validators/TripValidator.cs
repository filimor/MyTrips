using FluentValidation;
using MyTrips.Domain.Entities;

namespace MyTrips.Application.Validators;

public class TripValidator : AbstractValidator<Trip>
{
    public TripValidator()
    {
        RuleFor(x => x.ClientId)
            .GreaterThanOrEqualTo(1);
        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate);
        RuleFor(x => x.InboundFlightId)
            .GreaterThanOrEqualTo(1);
        RuleFor(x => x.OutboundFlightId)
            .GreaterThanOrEqualTo(1);
        RuleFor(x => x.HotelId)
            .GreaterThanOrEqualTo(1);
    }
}