namespace MyTrips.Presentation.Errors;

public class InternalServerErrorDetails : ErrorDetails
{
    public InternalServerErrorDetails(HttpContext context,
        string detail =
            "The server encountered an unexpected condition that prevented it from fulfilling the request. This is typically a temporary issue, and our team is working to resolve it as quickly as possible. Please try again later. If the problem persists, contact support for further assistance.")
    {
        Status = StatusCodes.Status500InternalServerError;
        Title = "Internal Server Error";
        Instance = context.Request.Path;
        Detail = detail;
    }
}