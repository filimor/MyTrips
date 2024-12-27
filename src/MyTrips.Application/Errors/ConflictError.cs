using FluentResults;

namespace MyTrips.Application.Errors;

public class ConflictError(string message) : Error(message);