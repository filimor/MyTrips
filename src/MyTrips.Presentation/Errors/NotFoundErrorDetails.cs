using FluentResults;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace MyTrips.Presentation.Errors;

public class NotFoundProblemDetails : ProblemDetails
{
    private NotFoundProblemDetails(HttpContext context, string? detail)
    {
        Status = StatusCodes.Status404NotFound;
        Title = "Not Found";
        Detail = detail ??
                 "The server cannot find the requested resource. This may be because the resource or any resources that it depends on does not exist, or the identifier was mistyped. Please check the sent data for errors.";
        Instance = context.Request.Path;
        Type = $"https://httpstatuses.com/{Status}";
    }

    public NotFoundProblemDetails(HttpContext context, ValidationResult validationResult,
        string? detail = null) : this(context, detail)
    {
        Extensions.Add("Errors", validationResult.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessage));
    }

    public NotFoundProblemDetails(HttpContext context, Result fluentResult,
        string? detail = null) : this(context, detail)
    {
        Extensions.Add("Errors", fluentResult.Errors.ToDictionary(e => e.Message, e => e.Reasons));
    }
}