namespace MyTrips.Presentation.Errors;

public class UnauthorizedErrorDetails : ErrorDetails
{
    public UnauthorizedErrorDetails(HttpContext context,
        string detail =
            "The credentials provided are invalid or missing. Please check your username and password and try again.")
    {
        Status = StatusCodes.Status401Unauthorized;
        Title = "Unauthorized";
        Instance = context.Request.Path;
        Detail = detail;
        Errors = [];
    }
}