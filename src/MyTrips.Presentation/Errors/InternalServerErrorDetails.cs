namespace MyTrips.Presentation.Errors;

public class InternalServerErrorDetails : ErrorDetails
{
    public InternalServerErrorDetails(HttpContext context,
        string detail = "An unexpected error occurred. Please try again later.")
    {
        Status = StatusCodes.Status500InternalServerError;
        Title = "Internal Server Error";
        Instance = context.Request.Path;
        Detail = detail;
    }
}