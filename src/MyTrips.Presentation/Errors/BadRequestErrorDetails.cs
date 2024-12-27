using FluentResults;

namespace MyTrips.Presentation.Errors;

public class BadRequestErrorDetails : ErrorDetails
{
    public BadRequestErrorDetails(HttpContext context, Result validationResult,
        string detail =
            "The server cannot process the request due to a client error. This could be due to a malformed request syntax, invalid request message framing, or deceptive request routing. Please check the request data and ensure it follows the correct format and try again.")
    {
        Status = StatusCodes.Status400BadRequest;
        Title = "Bad Request";
        Instance = context.Request.Path;
        Detail = detail;
        Errors = validationResult.Errors.Select(e => e.Message).ToArray();
    }
}