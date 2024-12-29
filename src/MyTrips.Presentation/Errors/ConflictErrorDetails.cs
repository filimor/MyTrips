using FluentResults;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace MyTrips.Presentation.Errors;

public class ConflictProblemDetails : ProblemDetails
{
    public ConflictProblemDetails(HttpContext context, string? detail)
    {
        Status = StatusCodes.Status409Conflict;
        Title = "Conflict";
        Detail = detail ??
                 "This operation cannot be executed because there is a conflict with the current state of the resource. This may be due to a conflicting update or an existing resource that conflicts with the requested operation. Please ensure that there are no conflicting changes or resources and try again.";
        Instance = context.Request.Path;
        Type = $"https://httpstatuses.com/{Status}";
    }

    public ConflictProblemDetails(HttpContext context, ValidationResult validationResult,
        string? detail = null) : this(context, detail)
    {
        Extensions.Add("Errors", validationResult.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessage));
    }

    public ConflictProblemDetails(HttpContext context, Result fluentResult,
        string? detail = null) : this(context, detail)
    {
        Extensions.Add("Errors", fluentResult.Errors.ToDictionary(e => e.Message, e => e.Reasons));
    }
}