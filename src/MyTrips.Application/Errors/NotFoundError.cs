using FluentResults;

namespace MyTrips.Application.Errors;

public class NotFoundError(string message) : Error(message);