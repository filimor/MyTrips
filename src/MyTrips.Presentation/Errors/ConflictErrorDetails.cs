using FluentResults;

namespace MyTrips.Presentation.Errors;

public class ConflictErrorDetails : ErrorDetails
{
    public ConflictErrorDetails(HttpContext context, Result validationResult,
        string detail =
            "This operation cannot be executed because there is a conflict with the current state of the resource. This may be due to a conflicting update or an existing resource that conflicts with the requested operation. Please ensure that there are no conflicting changes or resources and try again.")
    {
        Status = StatusCodes.Status409Conflict;
        Title = "Conflict";
        Instance = context.Request.Path;
        Detail = detail;
        Errors = validationResult.Errors.Select(e => e.Message).ToArray();
    }
}