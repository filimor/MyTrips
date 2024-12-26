using FluentResults;

namespace MyTrips.Presentation.Errors;

public class BadRequestErrorDetails : ErrorDetails
{
    public BadRequestErrorDetails(HttpContext context, Result validationResult,
        string detail = "Error validating data sent from client.")
    {
        Status = StatusCodes.Status400BadRequest;
        Title = "Bad Request";
        Instance = context.Request.Path;
        Detail = detail;
        Errors = validationResult.Errors.Select(e => e.Message).ToArray();
    }
}