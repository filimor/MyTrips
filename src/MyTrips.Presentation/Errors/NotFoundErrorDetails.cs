using FluentResults;

namespace MyTrips.Presentation.Errors;

public class NotFoundErrorDetails : ErrorDetails
{
    public NotFoundErrorDetails(HttpContext context, Result validationResult,
        string detail = "Request returned no results.")
    {
        Status = StatusCodes.Status404NotFound;
        Title = "Not Found";
        Instance = context.Request.Path;
        Detail = detail;
        Errors = validationResult.Errors.Select(e => e.Message).ToArray();
    }
}