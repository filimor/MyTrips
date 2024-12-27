using FluentResults;

namespace MyTrips.Presentation.Errors;

public class NotFoundErrorDetails : ErrorDetails
{
    public NotFoundErrorDetails(HttpContext context, Result validationResult,
        string detail =
            "The server cannot find the requested resource. This may be because the resource or any resources that it depends on does not exist, or the identifier was mistyped. Please check the sent data for errors.")
    {
        Status = StatusCodes.Status404NotFound;
        Title = "Not Found";
        Instance = context.Request.Path;
        Detail = detail;
        Errors = validationResult.Errors.Select(e => e.Message).ToArray();
    }
}